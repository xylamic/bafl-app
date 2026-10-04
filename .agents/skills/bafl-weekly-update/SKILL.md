---
name: bafl-weekly-update
description: 'Update BAFL football schedule scores and standings for a specific season and week from attached scoreboard/standings images. USE WHEN the user says things like "update BAFL 2026 week 1", "apply week 5 results", "record BAFL scores and standings", or attaches a weekly scoreboard image plus the five per-level standings images. Requires an explicit YEAR and WEEK. Edits archive/GameSchedule.<year>.json and archive/FootballStandings.<year>.json (never the .clean.json templates), following the formats in docs/game_schedule.md and docs/football_standings.md.'
argument-hint: '<year> week <n> (e.g. "2026 week 1") + attach scoreboard and 5 standings images'
---

# BAFL Weekly Update

Apply one week of game results and standings into the season's canonical JSON files from attached
images. The image transcription and formatting rules are intentionally NOT duplicated here — they
live in the reference docs. Read them before editing.

## Reference docs (read first)

- Schedule format and score rules: [game_schedule.md](../../../docs/game_schedule.md)
- Standings format and rank rules: [football_standings.md](../../../docs/football_standings.md)

## Required inputs

1. **Year and week**, stated explicitly (e.g. "2026 week 1"). If either is missing, ask before
   doing anything else — do not guess or infer from an image.
2. **One scoreboard image** for that week (columns per level: SR, JR, SOPH, FRESH, PW; rows are
   team pairs).
3. **Five standings images**, one per level (Peewee, Freshman, Sophomore, Junior, Senior), each
   with RANK, TEAM, WIN, LOSS, TIE, POINTS.

The standings images are the **source of truth** for standings. The schedule is used to
cross-check them.

## Target files

Resolve from the given YEAR:

- `archive/GameSchedule.<year>.json`
- `archive/FootballStandings.<year>.json`

Never edit `*.clean.json` (those are empty-season templates). State the two resolved paths and
confirm before editing.

## Procedure

1. **Confirm inputs.** Verify year, week, the scoreboard image, and all five standings images are
   present. If anything is missing, ask.
2. **Load the rules.** Read the two reference docs above for score orientation, forfeits, BYE
   handling, the levels order, `Points`, and competition ranking.
3. **Update the schedule** (`archive/GameSchedule.<year>.json`):
   - Locate the matching `Week` entry.
   - For each pair of teams on the scoreboard, find that week's matchup and match the image's
     mascots to the matchup `Home`/`Away` full names. Abbreviations: `M. Texans` = Manvel Texans,
     `P. Texans` = Pearland Texans, `LP Texans` = La Porte Texans.
   - Write each level's `Score` as `"<away> @ <home>"` (away score first). Forfeits are `1`–`0`
     (e.g. `"1 @ 0"` or `"0 @ 1"`). Leave any level that did not play as `"TBA"`.
   - Do not modify BYE entries.
4. **Update the standings** (`archive/FootballStandings.<year>.json`):
   - For each level image, set every team's `Wins`, `Losses`, `Ties`, `Playoff`, and `Rank`
     exactly as shown. Set `Points = Wins + 0.5 × Ties`.
   - Ranks use competition ranking (tied teams share a rank; next rank = `1 + teams ahead`).
5. **Set the message.** On both files set `"Message"` to `"Week <n> updated."`.
6. **Cross-check integrity.** Run the derivation check; investigate any mismatch. The standings
   images win, but a mismatch usually means a mis-transcribed score or standings cell — fix the
   real error rather than masking it:

   ```
   python3 .agents/skills/bafl-weekly-update/scripts/derive_standings.py \
       archive/GameSchedule.<year>.json archive/FootballStandings.<year>.json
   ```

7. **Validate structure.** Must exit 0:

   ```
   python3 .agents/skills/bafl-weekly-update/scripts/validate.py \
       archive/GameSchedule.<year>.json archive/FootballStandings.<year>.json
   ```

8. **Report.** Summarize the games entered and the standings changes, and note any BYE or
   unplayed-level cases.

## Scripts

- [scripts/validate.py](./scripts/validate.py) — structural + consistency checks (valid JSON,
  five levels, all teams present once per week, score format, `Points` formula, rank sanity).
  Exits non-zero on any problem.
- [scripts/derive_standings.py](./scripts/derive_standings.py) — read-only integrity check that
  derives cumulative W/L/T from the schedule and diffs against the standings file. Never rewrites
  standings; the images remain authoritative.

## Guardrails

- Require an explicit year and week; ask if missing.
- Only edit the two `archive/*.<year>.json` files; never `*.clean.json`.
- Away score is always written first in schedule scores.
- Keep the five levels in order: Peewee, Freshman, Sophomore, Junior, Senior.
