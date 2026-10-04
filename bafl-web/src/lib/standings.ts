// Mirrors bafl-app/Library/BaflStandingTeam.cs and StandingsView.xaml.cs. Spec: docs/football_standings.md.
import type { StandingTeam } from './types';

/** Wins + 0.5 × Ties. The payload's `Points` field is informational and ignored. */
export function teamPoints(team: StandingTeam): number {
	return team.Wins + 0.5 * team.Ties;
}

/** Rank ascending, then team name; ties in rank share a position. */
export function sortStandingTeams(teams: readonly StandingTeam[]): StandingTeam[] {
	return [...teams].sort((a, b) => a.Rank - b.Rank || a.Team.localeCompare(b.Team));
}
