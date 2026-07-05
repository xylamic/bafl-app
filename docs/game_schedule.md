# Game Schedule JSON Specification

This document defines the layout and semantics of the `GameSchedule.<year>.json` files
(e.g. [archive/GameSchedule.2026.json](../archive/GameSchedule.2026.json)) consumed by the
BAFL app's schedule view. It describes the season-long lifecycle of a schedule file — from a
freshly generated regular season through playoffs, the All-Star Game, and the Super Bowl — so
that score entry can be automated reliably.

The schema maps directly onto the C# model classes in
[bafl-app/Library](../bafl-app/Library): `BaflGameCalendar`, `BaflGameWeek`,
`BaflGameMatchup`, and `BaflGameMatchupScore`. Field names are case-sensitive and must match
exactly.

## File naming and location

- Live season files are served from a remote endpoint and deserialized into `BaflGameCalendar`.
- Completed seasons are archived as `archive/GameSchedule.<year>.json`.
- One file represents one full season, including postseason weeks.

## Top-level object (`BaflGameCalendar`)

```json
{
  "Title": "2026 Schedule",
  "Message": "",
  "Weeks": [ /* array of week objects */ ]
}
```

| Field     | Type     | Required | Notes |
|-----------|----------|----------|-------|
| `Title`   | string   | yes      | Display title, e.g. `"2026 Schedule"`. |
| `Message` | string   | yes      | Optional banner shown above the schedule. Empty string hides it. Use it for season-wide notices (e.g. `"Week 11 updated. Playoffs begin."`). |
| `Weeks`   | array    | yes      | Ordered list of week objects. The app auto-selects the week whose date is within two days of today, so keep `Weeks` in chronological order. |

## Week object (`BaflGameWeek`)

```json
{
  "Week": "Week 1",
  "Date": "2026-08-15",
  "Matchups": [ /* array of matchup objects */ ]
}
```

| Field      | Type   | Required | Notes |
|------------|--------|----------|-------|
| `Week`     | string | yes      | Free-form label. Regular season uses `"Week 1"` … `"Week 11"`. Postseason uses `"Playoff Round 1"`, `"Playoff Round 2"`, `"BAFL All Star Game"`, `"Super Bowl"`. |
| `Date`     | string | yes      | ISO date `yyyy-MM-dd`. Only month and day are displayed; the year is not rendered but should still be correct. Drives the "closest week" auto-select. |
| `Matchups` | array  | yes      | List of matchup objects for that week. |

## Matchup object (`BaflGameMatchup`)

```json
{
  "Home": "Angleton Wildcats",
  "Away": "Sagemont Cowboys",
  "IsNeutral": false,
  "Details": "",
  "Scores": [ /* array of score objects */ ]
}
```

| Field       | Type    | Required | Default | Notes |
|-------------|---------|----------|---------|-------|
| `Home`      | string  | yes      | —       | Home team full name (`"<Region> <Mascot>"`). See special values below. |
| `Away`      | string  | yes      | —       | Away/visiting team full name. |
| `IsNeutral` | bool    | no       | `false` | When `true`, the app shows the divider `vs` instead of `@`. Used for the All-Star Game and Super Bowl (played at neutral sites). |
| `Details`   | string  | no       | `""`    | Optional italic note under the matchup. Used for venue text and, in the Super Bowl, the champion (prefixed with 🏆). Empty string hides it. |
| `Scores`    | array   | no       | `[]`    | Level/score rows (see below). Omitted entirely for BYE entries. |

`IsNeutral` and `Details` may be omitted for regular-season games; the deserializer fills the
defaults. The generated new-season file omits both for brevity.

### Display convention

The app renders a matchup as the away team on top, then a divider, then the home team:

```
        Away Team
      @ Home Team          (divider is "vs" when IsNeutral is true)
   <score row>  <score row>  ...
   Details (italic, if present)
```

The `@` divider means "away **at** home," the standard "visitor @ home" notation.

### Special value: BYE

A team on bye is recorded as a matchup with `Home` set to the literal `"BYE"` and the bye team
in `Away`, with **no** `Scores` array:

```json
{ "Home": "BYE", "Away": "Bay Area Buccaneers" }
```

One such entry is added per team on bye that week. In an 18-team season, weeks with an even
split have nine games and no BYE entries; weeks with byes have fewer games plus one BYE entry
per idle team.

## Score object (`BaflGameMatchupScore`)

```json
{ "Level": "Peewee", "Score": "TBA" }
```

| Field   | Type   | Required | Notes |
|---------|--------|----------|-------|
| `Level` | string | yes      | Age level or, in the postseason, a repurposed label (see below). |
| `Score` | string | yes      | Free-form string. Meaning depends on the week type (see below). |

Both fields are plain strings. The app performs no numeric parsing — it displays `Level`, a
space, then `Score` verbatim. All formatting rules below are conventions enforced by the data,
not the app.

## Regular season (Weeks 1–11)

Each non-BYE matchup carries exactly five score rows, in this order:

```
Peewee, Freshman, Sophomore, Junior, Senior
```

### Before a game is played

`Score` is the literal string `"TBA"`:

```json
{
  "Home": "Angleton Wildcats",
  "Away": "Sagemont Cowboys",
  "Scores": [
    { "Level": "Peewee",    "Score": "TBA" },
    { "Level": "Freshman",  "Score": "TBA" },
    { "Level": "Sophomore", "Score": "TBA" },
    { "Level": "Junior",    "Score": "TBA" },
    { "Level": "Senior",    "Score": "TBA" }
  ]
}
```

A newly generated season file (such as `GameSchedule.2026.json`) sets every regular-season
score to `"TBA"` and contains **no** postseason weeks — those are appended once the regular
season concludes.

### After a game is played

Replace `"TBA"` with the final score using the format:

```
<away score> @ <home score>
```

The number **before** the `@` is the **away** team's score; the number **after** is the
**home** team's score. This mirrors the "Away @ Home" display order. Example: for
`Home = "Texas City Stingrays"`, `Away = "Angleton Wildcats"`, a score of `"24 @ 7"` means
Angleton (away) 24, Texas City (home) 7.

> Orientation verified against the 2025 season: applying away-first parsing reproduces the
> published standings exactly (e.g. Angleton Peewee 10–0, Dickinson Peewee 0–10).

To automate score entry, match a played game to its matchup by (`Week`, `Home`, `Away`), then
set each level's `Score` to `"<away> @ <home>"`. Leave levels that did not play as `"TBA"`.

## Postseason weeks

Postseason weeks reuse the same object shapes but repurpose several fields. Automation should
branch on the `Week` label.

### Playoff rounds (`"Playoff Round 1"`, `"Playoff Round 2"`)

In playoff weeks the fields are repurposed to publish a game-day time grid rather than
head-to-head scores:

- `Home` / `Away` hold bracket/grouping labels (e.g. `"Pearland Texans"` / `"Seniors"`), not a
  single game.
- Each `Scores` row uses `Level` as a **matchup descriptor** and `Score` as the **kickoff
  time**.

```json
{
  "Home": "Pearland Texans",
  "Away": "Seniors",
  "IsNeutral": false,
  "Details": "",
  "Scores": [
    { "Level": "Pw: Pearland T vs La Porte", "Score": "8:00am" },
    { "Level": "Sagemont vs Hitchcock",      "Score": "9:00am" },
    { "Level": "Pearland T vs Bay Area B",   "Score": "11:00am" }
  ]
}
```

Because the semantics differ from the regular season, do not apply the `"<away> @ <home>"`
score rule here.

### All-Star Game (`"BAFL All Star Game"`)

A single neutral-site matchup with time slots per level:

```json
{
  "Home": "Red",
  "Away": "Blue",
  "IsNeutral": true,
  "Details": "The Rig - 3775 Main St., Pearland, TX",
  "Scores": [
    { "Level": "Peewees",    "Score": "11:00am" },
    { "Level": "Freshmen",   "Score": "12:30pm" },
    { "Level": "Sophomores", "Score": "2:00pm" },
    { "Level": "Powder Puff", "Score": "3:30pm" },
    { "Level": "Juniors",    "Score": "5:00pm" },
    { "Level": "Seniors",    "Score": "7:30pm" }
  ]
}
```

- `IsNeutral` is `true` (renders `Red vs Blue`).
- `Details` holds the venue.
- `Score` holds a kickoff time, not a final score.

### Super Bowl (`"Super Bowl"`)

One matchup per level, each at a neutral site, with a final score:

```json
{
  "Home": "Angleton Wildcats",
  "Away": "Bay Area Cardinals",
  "IsNeutral": true,
  "Details": "🏆 Angleton Wildcats",
  "Scores": [
    { "Level": "PW: ", "Score": "18 - 39 Final" }
  ]
}
```

- `IsNeutral` is `true`.
- `Details` names the champion, prefixed with the 🏆 emoji.
- Each matchup carries a single `Scores` row; `Level` is the level tag (e.g. `"PW:"`, `"FR:"`)
  and `Score` is the final score in the format `"<away> - <home> Final"` (space-hyphen-space
  separator, away first, home second, followed by `Final`). In the example, Angleton (home)
  won 39–18, consistent with the `Details` champion.

## Field summary by week type

| Week type      | `IsNeutral` | `Details`            | `Level` holds            | `Score` holds                 |
|----------------|-------------|----------------------|--------------------------|-------------------------------|
| Regular season | `false`     | usually empty        | age level                | `"TBA"` or `"<away> @ <home>"` |
| Playoff round  | `false`     | usually empty        | matchup descriptor       | kickoff time (e.g. `"9:00am"`) |
| All-Star Game  | `true`      | venue                | level (plural)           | kickoff time                  |
| Super Bowl     | `true`      | `"🏆 <champion>"`    | level tag (e.g. `"PW:"`) | `"<away> - <home> Final"`     |

## Automation checklist

1. Parse the file and locate the target `Week` by its label.
2. Regular season: identify the game by (`Home`, `Away`); for each level that played, set
   `Score` to `"<away> @ <home>"`. Away score is written first.
3. Never alter BYE entries (`Home == "BYE"`) — they have no `Scores`.
4. Keep the five regular-season levels in order: Peewee, Freshman, Sophomore, Junior, Senior.
5. Do not apply the regular-season score rule to playoff, All-Star, or Super Bowl weeks; those
   use times or the `"<away> - <home> Final"` form as documented above.
6. Preserve `Weeks` in chronological order so the app's "closest week" selection works.
