#!/usr/bin/env python3
"""Integrity check: derive cumulative standings from the schedule and diff against the
standings file. Read-only — it never rewrites the standings.

Usage:
    python3 derive_standings.py <GameSchedule.json> <FootballStandings.json>

The standings images are the source of truth. This tool only flags where the schedule-derived
win/loss/tie records disagree with the standings file, so transcription mistakes surface.
Exits 0 when records match, 1 when there are unexplained differences.

Score orientation is away-first: "<away> @ <home>" (see docs/game_schedule.md).
"""
import json
import re
import sys

LEVELS = ["Peewee", "Freshman", "Sophomore", "Junior", "Senior"]
SCORE_RE = re.compile(r"^\s*(\d+)\s*@\s*(\d+)\s*$")


def load(path):
    with open(path) as f:
        return json.load(f)


def derive(schedule):
    """Return {level: {team: [W, L, T]}} accumulated over all played (non-TBA) games."""
    rec = {lvl: {} for lvl in LEVELS}
    for wk in schedule["Weeks"]:
        # Only regular-season weeks carry head-to-head level scores.
        if not str(wk.get("Week", "")).lower().startswith("week"):
            continue
        for m in wk["Matchups"]:
            if m.get("Home") == "BYE":
                continue
            home, away = m["Home"], m["Away"]
            for s in m.get("Scores", []):
                match = SCORE_RE.match(s.get("Score", ""))
                if not match:
                    continue
                away_pts, home_pts = int(match.group(1)), int(match.group(2))
                lvl = s.get("Level")
                if lvl not in rec:
                    continue
                for team, mine, theirs in [(away, away_pts, home_pts), (home, home_pts, away_pts)]:
                    r = rec[lvl].setdefault(team, [0, 0, 0])
                    if mine > theirs:
                        r[0] += 1
                    elif mine < theirs:
                        r[1] += 1
                    else:
                        r[2] += 1
    return rec


def main():
    if len(sys.argv) != 3:
        print(__doc__)
        return 2
    schedule = load(sys.argv[1])
    standings = load(sys.argv[2])
    derived = derive(schedule)

    diffs = []
    for entry in standings["Standings"]:
        lvl = entry.get("Level")
        for t in entry.get("Teams", []):
            name = t.get("Team")
            have = (t.get("Wins", 0), t.get("Losses", 0), t.get("Ties", 0))
            want = tuple(derived.get(lvl, {}).get(name, [0, 0, 0]))
            if have != want:
                diffs.append(
                    f"{lvl}: {name} standings W-L-T {have} != schedule-derived {want}"
                )

    if diffs:
        print(f"MISMATCH: {len(diffs)} record(s) differ between standings and schedule")
        for d in diffs:
            print("  -", d)
        print("\nStandings images are authoritative; investigate whether the schedule scores "
              "or the transcribed standings need correcting.")
        return 1
    print("OK: standings records match the schedule-derived records")
    return 0


if __name__ == "__main__":
    sys.exit(main())
