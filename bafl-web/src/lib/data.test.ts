import { describe, expect, it } from 'vitest';
import { parseCoreInfo } from './api';
import { parseLocalDate } from './dates';
import { byeTeam, findClosestWeekIndex, isBye, matchupDivider, weekLabel } from './schedule';
import { sortStandingTeams, teamPoints } from './standings';
import type { GameWeek, StandingTeam } from './types';

const weeks: GameWeek[] = ['2026-08-15', '2026-08-22', '2026-09-26', '2026-10-03', '2026-10-10'].map((Date, i) => ({
	Week: `Week ${i + 1}`,
	Date,
	Matchups: []
}));

describe('parseLocalDate', () => {
	it('reads date-only values as local midnight, not UTC', () => {
		const d = parseLocalDate('2026-08-15')!;
		expect([d.getFullYear(), d.getMonth(), d.getDate(), d.getHours()]).toEqual([2026, 7, 15, 0]);
	});

	it('reads offset-less date-times as local time', () => {
		const d = parseLocalDate('2026-08-22T18:30:00')!;
		expect([d.getDate(), d.getHours(), d.getMinutes()]).toEqual([22, 18, 30]);
	});

	it('returns null for empty or invalid input', () => {
		expect(parseLocalDate('')).toBeNull();
		expect(parseLocalDate(undefined)).toBeNull();
		expect(parseLocalDate('nope')).toBeNull();
	});
});

describe('findClosestWeekIndex', () => {
	it('keeps showing a week until 2 days after its date', () => {
		expect(findClosestWeekIndex(weeks, new Date(2026, 9, 4, 12))).toBe(3);
		expect(findClosestWeekIndex(weeks, new Date(2026, 9, 4, 23, 59))).toBe(3);
		expect(findClosestWeekIndex(weeks, new Date(2026, 9, 5, 0, 0))).toBe(4);
	});

	it('starts at the first week before the season', () => {
		expect(findClosestWeekIndex(weeks, new Date(2026, 6, 1))).toBe(0);
	});

	it('falls back to the last week after the season', () => {
		expect(findClosestWeekIndex(weeks, new Date(2027, 0, 1))).toBe(4);
	});

	it('returns -1 for no weeks', () => {
		expect(findClosestWeekIndex([], new Date())).toBe(-1);
	});
});

describe('matchup display', () => {
	it('labels weeks like iOS', () => {
		expect(weekLabel(weeks[2]!)).toBe('Week 3, September 26');
	});

	it('uses "@" for home games and "vs" for neutral sites', () => {
		expect(matchupDivider({ Home: 'A', Away: 'B' })).toBe('@');
		expect(matchupDivider({ Home: 'A', Away: 'B', IsNeutral: true })).toBe('vs');
	});

	it('detects BYE entries and the idle team', () => {
		const bye = { Home: 'BYE', Away: 'Bay Area Buccaneers' };
		expect(isBye(bye)).toBe(true);
		expect(byeTeam(bye)).toBe('Bay Area Buccaneers');
		expect(isBye({ Home: 'A', Away: 'B' })).toBe(false);
	});
});

describe('standings', () => {
	const team = (Team: string, Rank: number, Wins = 0, Ties = 0): StandingTeam => ({
		Team,
		Rank,
		Wins,
		Losses: 0,
		Ties,
		Playoff: false
	});

	it('derives points as wins plus half ties', () => {
		expect(teamPoints(team('A', 1, 6, 1))).toBe(6.5);
		expect(teamPoints({ ...team('A', 1, 2), Points: 99 })).toBe(2);
	});

	it('sorts by rank, then name', () => {
		const sorted = sortStandingTeams([team('Zeta', 2), team('Beta', 1), team('Alpha', 2)]);
		expect(sorted.map((t) => t.Team)).toEqual(['Beta', 'Alpha', 'Zeta']);
	});
});

describe('parseCoreInfo', () => {
	it('double-parses the nested JSON strings', () => {
		const raw = {
			teams: JSON.stringify({ '1': { Region: 'Angleton', Football: 'Wildcats' } }),
			board: JSON.stringify([{ Role: 'President', Name: 'X', Email: '' }]),
			schedule: JSON.stringify([{ Date: '2026-08-15', Name: 'Week 1', Location: 'Fields' }])
		};
		const info = parseCoreInfo(raw);
		expect(info.teams['1']?.Region).toBe('Angleton');
		expect(info.board[0]?.Role).toBe('President');
		expect(info.schedule[0]?.Name).toBe('Week 1');
	});
});
