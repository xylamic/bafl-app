# Football Standings JSON Specification

This document defines the layout and semantics of the `FootballStandings.<year>.json` files
(e.g. [archive/FootballStandings.2026.json](../archive/FootballStandings.2026.json)) consumed
by the BAFL app's standings view. It covers the structure, field meanings, and the conventions
used to seed a clean season and update records so standings can be automated.

The schema maps onto the C# model classes in [bafl-app/Library](../bafl-app/Library):
`BaflStandings`, `BaflStandingEntry`, and `BaflStandingTeam`. Field names are case-sensitive
and must match exactly.

## File naming and location

- Live season files are served from a remote endpoint and deserialized into `BaflStandings`.
- Completed seasons are archived as `archive/FootballStandings.<year>.json`.
- One file holds the standings for every age level in a single season.

## Top-level object (`BaflStandings`)

```json
{
  "Title": "2026 Standings",
  "Message": "",
  "Standings": [ /* array of level entries */ ]
}
```

| Field       | Type   | Required | Notes |
|-------------|--------|----------|-------|
| `Title`     | string | yes      | Display title, e.g. `"2026 Standings"`. |
| `Message`   | string | yes      | Optional banner shown above the standings. Empty string hides it. Use it for update notices (e.g. `"Week 11 updated. Playoffs begin."`). |
| `Standings` | array  | yes      | One entry per age level. |

## Level entry (`BaflStandingEntry`)

```json
{
  "Level": "Peewee",
  "Teams": [ /* array of team standing objects */ ]
}
```

| Field   | Type   | Required | Notes |
|---------|--------|----------|-------|
| `Level` | string | yes      | Age level. The season uses five, in this order: `Peewee`, `Freshman`, `Sophomore`, `Junior`, `Senior`. |
| `Teams` | array  | yes      | Standing rows for every team competing at that level. |

Each level lists the full set of teams for the season (18 in 2026). The same team appears once
per level.

## Team standing object (`BaflStandingTeam`)

```json
{
  "Team": "Angleton Wildcats",
  "Wins": 0,
  "Losses": 0,
  "Ties": 0,
  "Points": 0,
  "Playoff": false,
  "Rank": 1
}
```

| Field     | Type    | Required | Notes |
|-----------|---------|----------|-------|
| `Team`    | string  | yes      | Full team name, `"<Region> <Mascot>"`, matching the names in `Teams.<year>.json` and `GameSchedule.<year>.json`. |
| `Wins`    | integer | yes      | Win count. |
| `Losses`  | integer | yes      | Loss count. |
| `Ties`    | integer | yes      | Tie count. |
| `Points`  | number  | see note | Standings points. **The app ignores this field on load and recomputes it** (see below). Keep it consistent with the formula for readability. |
| `Playoff` | boolean | yes      | `true` marks the team as in a playoff position; the app shows a ⭐️ next to the name. |
| `Rank`    | integer | yes      | Standing position within the level. Drives sort order. |

### Points is computed, not read

`BaflStandingTeam.Points` is a read-only computed property:

```
Points = Wins + (0.5 × Ties)
```

The deserializer has no setter for `Points`, so whatever value the JSON carries is discarded and
the app recomputes it from `Wins` and `Ties`. The field is still written into the file for human
readability and to match the historical format; when set, it should equal `Wins + 0.5 × Ties`.
A newly seeded file uses `0`.

### Display and sorting

Within a selected level, the view sorts teams by `Rank` ascending, then by `Team` name
alphabetically as a tiebreaker. Each row renders:

- `Rank`, then `Team`, then ⭐️ when `Playoff` is `true`.
- `Wins` as `"<n>W"`, `Losses` as `"<n>L"`, `Ties` as `"<n>T"`, and points as `"<n>Pts"`.

Because sorting is by `Rank`, ranks should be assigned so the intended order is preserved. When
every team shares `Rank = 1` (a freshly seeded file), the view falls back to alphabetical order
by team name.

## Clean-season seed format

A new season file (such as `FootballStandings.2026.json`) is seeded as follows:

- `Title` is `"<year> Standings"`; `Message` is `""`.
- One entry per level in order: Peewee, Freshman, Sophomore, Junior, Senior.
- Every team from `Teams.<year>.json` appears in each level, in the team-list order.
- For every team: `Wins`, `Losses`, `Ties`, and `Points` are `0`; `Playoff` is `false`;
  `Rank` is `1`.

```json
{
  "Team": "Angleton Wildcats",
  "Wins": 0,
  "Losses": 0,
  "Ties": 0,
  "Points": 0,
  "Playoff": false,
  "Rank": 1
}
```

## Automation checklist

1. Parse the file and locate the target level by its `Level` label.
2. For each team, set `Wins`, `Losses`, and `Ties` from the season results at that level.
3. Set `Points` to `Wins + 0.5 × Ties` for consistency (the app recomputes this regardless).
4. Recompute `Rank` per level from the standings order (typically by points, then tiebreakers),
   assigning `1` to the top team. The view sorts on `Rank`, so it must reflect the intended order.
5. Set `Playoff` to `true` for teams in a qualifying position; the app renders a ⭐️ for them.
6. Keep team names identical across `Teams.<year>.json`, `GameSchedule.<year>.json`, and this
   file so records line up.
7. Optionally set `Message` to a short update note (e.g. `"Week 5 updated."`); leave it `""` to
   hide the banner.
```
