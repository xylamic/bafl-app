# BAFL Azure Data Integration

This document describes how to consume the BAFL Azure Functions backend so another
application can retrieve the same league data used by the BAFL mobile app. All
endpoints are HTTP-triggered Azure Functions hosted at:

```
https://baflapp.azurewebsites.net/api
```

## Authentication

Every data endpoint (except `app-config`) is protected by an Azure Functions access
key passed as a `code` query-string parameter. The key is not hard-coded in the
client; instead it is fetched at runtime from the unauthenticated `app-config`
endpoint.

### Step 1 — Get the access key

`POST /api/app-config`

Send a JSON body identifying the calling context:

```json
{ "context": "BaflApp" }
```

Response:

```json
{ "Key": "<access-key>" }
```

### Step 2 — Call data endpoints with the key

Append the key to every subsequent request:

```
GET /api/coreinfo?code=<access-key>
```

Notes for integrators:
- Cache the key for the session; only re-fetch it if a request fails auth.
- Reuse a single HTTP client instance to avoid socket exhaustion.
- The key is an app-level shared secret, not a per-user credential. Treat it as
  sensitive and do not embed it in public source or logs.

## Endpoints

| Purpose | Method | Path | Auth | Response type |
|---|---|---|---|---|
| Get access key | POST | `/api/app-config` | none | `{ "Key": string }` |
| Core info (teams, board, schedule) | GET | `/api/coreinfo` | `code` | object of JSON strings |
| Teams only | GET | `/api/teams` | `code` | map of clubs |
| Board only | GET | `/api/board` | `code` | list of board members |
| Season schedule only | GET | `/api/schedule` | `code` | list of schedule items |
| Cheer competition | GET | `/api/cheercomp` | `code` | event |
| Drill competition | GET | `/api/drillcomp` | `code` | event |
| Game calendar (11v11) | GET | `/api/calendar` | `code` | game calendar |
| Game calendar (9v9) | GET | `/api/calendar9v9` | `code` | game calendar |
| Standings (11v11) | GET | `/api/standings` | `code` | standings |
| Standings (9v9) | GET | `/api/standings9v9` | `code` | standings |

## Response Schemas

### Core info — `GET /api/coreinfo`

Returns a JSON object whose values are themselves JSON-encoded strings. Each value
must be parsed a second time to obtain the nested structure.

```json
{
  "teams": "<json string: map of clubs>",
  "board": "<json string: list of board members>",
  "schedule": "<json string: list of schedule items>"
}
```

The `teams`, `board`, and `schedule` payloads match the standalone `/api/teams`,
`/api/board`, and `/api/schedule` endpoints described below.

### Teams — `GET /api/teams`

A map keyed by integer club ID.

```json
{
  "1": {
    "Region": "Pearland",
    "Football": "Texans",
    "Cheer": "Texans",
    "Mascot": "Texan",
    "Website": "https://example.org",
    "President": "Jane Doe",
    "FieldName": "Home Field",
    "FieldLocation": "123 Main St"
  }
}
```

### Board — `GET /api/board`

```json
[
  { "Role": "President", "Name": "Jane Doe", "Email": "president@example.org" }
]
```

`Email` may be empty.

### Season schedule — `GET /api/schedule`

```json
[
  { "Date": "2026-08-15T18:00:00", "Name": "Season Kickoff", "Location": "Main Field", "Notable": true }
]
```

### Cheer / Drill competition — `GET /api/cheercomp`, `GET /api/drillcomp`

```json
{
  "Name": "2026 Cheer Competition",
  "Date": "2026-11-01T00:00:00",
  "Message": "",
  "Information": "General info text",
  "DoorsOpen": "8:00AM",
  "MoreInfo": "https://example.org/info",
  "Tickets": "https://example.org/tickets",
  "Schedule": [
    {
      "Group": "Peewee",
      "Name": "Team A Performance",
      "ScheduledStart": "9:00AM",
      "Status": "",
      "Highlight": false,
      "Notable": false
    }
  ]
}
```

### Game calendar — `GET /api/calendar`, `GET /api/calendar9v9`

```json
{
  "Title": "2026 Game Schedule",
  "Message": "",
  "Weeks": [
    {
      "Week": "Week 1",
      "Date": "2026-08-22T00:00:00",
      "Matchups": [
        {
          "Home": "Pearland Texans",
          "Away": "Dickinson Gators",
          "IsNeutral": false,
          "Details": "",
          "Scores": [
            { "Level": "Peewee", "Score": "14-6" }
          ]
        }
      ]
    }
  ]
}
```

`IsNeutral` indicates a neutral-site game (no true home team). `Scores` is empty
until results are posted.

### Standings — `GET /api/standings`, `GET /api/standings9v9`

```json
{
  "Title": "2026 Standings",
  "Message": "",
  "Standings": [
    {
      "Level": "Peewee",
      "Teams": [
        {
          "Team": "Pearland Texans",
          "Wins": 0,
          "Losses": 0,
          "Ties": 0,
          "Points": 0,
          "Playoff": false,
          "Rank": 1
        }
      ]
    }
  ]
}
```

The `Points` field is derived in the app as `Wins + 0.5 * Ties`; the source `Points`
value in the payload may be present but is treated as informational.

## Typical Integration Flow

1. `POST /api/app-config` with `{ "context": "BaflApp" }` to obtain the key.
2. `GET /api/coreinfo?code=<key>` for teams, board, and season schedule in one call,
   then double-parse each nested value.
3. `GET /api/calendar?code=<key>` and `GET /api/standings?code=<key>` (and their
   `9v9` variants) for live game data.
4. `GET /api/cheercomp?code=<key>` and `GET /api/drillcomp?code=<key>` for
   competition events.

## Error Handling

- On any non-success response, re-fetch the key via `app-config` and retry once, in
  case the key rotated.
- Endpoints return standard HTTP status codes; a `401`/`403` indicates a missing or
  invalid `code` parameter.
- Payloads are UTF-8 JSON. Date fields are ISO 8601 strings.
