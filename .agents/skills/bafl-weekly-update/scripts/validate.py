#!/usr/bin/env python3
"""Structural + consistency validation for BAFL schedule and standings JSON files.

Usage:
    python3 validate.py <GameSchedule.json> <FootballStandings.json>

Exits 0 when all checks pass, 1 otherwise. Prints a per-check report.
Rules are defined in docs/game_schedule.md and docs/football_standings.md.
"""
import json
import re
import sys

LEVELS = ["Peewee", "Freshman", "Sophomore", "Junior", "Senior"]
SCORE_RE = re.compile(r"^\s*\d+\s*@\s*\d+\s*$")


def load(path):
    with open(path) as f:
        return json.load(f)


def team_names(schedule):
    names = set()
    for wk in schedule["Weeks"]:
        for m in wk["Matchups"]:
            if m["Home"] != "BYE":
                names.add(m["Home"])
            names.add(m["Away"])
    return names


def validate_schedule(schedule, errors):
    teams = team_names(schedule)
    for wk in schedule["Weeks"]:
        label = wk.get("Week", "?")
        if not wk.get("Week") or not wk.get("Date"):
            errors.append(f"schedule: week missing Week/Date near '{label}'")
        seen = []
        for m in wk["Matchups"]:
            home, away = m.get("Home", ""), m.get("Away", "")
            if home == "BYE":
                seen.append(away)
                if "Scores" in m and m["Scores"]:
                    errors.append(f"schedule {label}: BYE for {away} should have no Scores")
                continue
            seen.extend([home, away])
            for s in m.get("Scores", []):
                sc = s.get("Score", "")
                if sc != "TBA" and not SCORE_RE.match(sc):
                    errors.append(
                        f"schedule {label}: bad score '{sc}' for {away} @ {home} "
                        f"({s.get('Level')}); expected '<away> @ <home>' or 'TBA'"
                    )
        dups = {t for t in seen if seen.count(t) > 1}
        if dups:
            errors.append(f"schedule {label}: team(s) appear more than once: {sorted(dups)}")
        missing = teams - set(seen)
        if missing:
            errors.append(f"schedule {label}: missing team(s): {sorted(missing)}")
    return teams


def validate_standings(standings, schedule_teams, errors):
    levels = [e.get("Level") for e in standings["Standings"]]
    if levels != LEVELS:
        errors.append(f"standings: levels {levels} != expected {LEVELS}")
    for entry in standings["Standings"]:
        lvl = entry.get("Level", "?")
        names = [t.get("Team") for t in entry.get("Teams", [])]
        if schedule_teams and set(names) != schedule_teams:
            extra = set(names) - schedule_teams
            missing = schedule_teams - set(names)
            if extra:
                errors.append(f"standings {lvl}: unexpected team(s): {sorted(extra)}")
            if missing:
                errors.append(f"standings {lvl}: missing team(s): {sorted(missing)}")
        if len(names) != len(set(names)):
            errors.append(f"standings {lvl}: duplicate team rows")
        for t in entry.get("Teams", []):
            w, l, tie = t.get("Wins", 0), t.get("Losses", 0), t.get("Ties", 0)
            pts = t.get("Points", 0)
            expected = w + 0.5 * tie
            if pts != expected:
                errors.append(
                    f"standings {lvl}: {t.get('Team')} Points {pts} != Wins+0.5*Ties {expected}"
                )
            if t.get("Rank", 0) < 1:
                errors.append(f"standings {lvl}: {t.get('Team')} Rank must be >= 1")
        # competition-ranking sanity: sort by points desc, ranks non-decreasing, min rank 1
        ordered = sorted(entry.get("Teams", []), key=lambda x: -(x.get("Wins", 0) + 0.5 * x.get("Ties", 0)))
        ranks = [t.get("Rank", 0) for t in ordered]
        if ranks and min(ranks) != 1:
            errors.append(f"standings {lvl}: top rank should be 1 (got min {min(ranks)})")


def main():
    if len(sys.argv) != 3:
        print(__doc__)
        return 2
    schedule = load(sys.argv[1])
    standings = load(sys.argv[2])
    errors = []
    teams = validate_schedule(schedule, errors)
    validate_standings(standings, teams, errors)
    if errors:
        print(f"FAIL: {len(errors)} issue(s)")
        for e in errors:
            print("  -", e)
        return 1
    print("PASS: schedule and standings are structurally valid and consistent")
    return 0


if __name__ == "__main__":
    sys.exit(main())
