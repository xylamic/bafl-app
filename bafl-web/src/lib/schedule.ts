// Mirrors bafl-app/ScheduleView.xaml.cs and bafl-app/Library/BaflGame*.cs. Format spec: docs/game_schedule.md.
import { addDays, formatMonthDay, parseLocalDate } from './dates';
import type { GameWeek, Matchup } from './types';

/**
 * The week to show first: the first week whose date plus 2 days is still in the future,
 * otherwise the last week. Returns -1 for an empty schedule.
 */
export function findClosestWeekIndex(weeks: readonly GameWeek[], now: Date = new Date()): number {
	for (let i = 0; i < weeks.length; i++) {
		const date = parseLocalDate(weeks[i]?.Date);
		if (date && addDays(date, 2) > now) return i;
	}
	return weeks.length - 1;
}

/** "Week 7, September 26" */
export function weekLabel(week: GameWeek): string {
	return `${week.Week}, ${formatMonthDay(parseLocalDate(week.Date))}`;
}

/** "@" for a home game, "vs" at a neutral site. */
export function matchupDivider(matchup: Matchup): string {
	return matchup.IsNeutral ? 'vs' : '@';
}

export function isBye(matchup: Matchup): boolean {
	return matchup.Home === 'BYE' || matchup.Away === 'BYE';
}

/** The idle team in a BYE entry. */
export function byeTeam(matchup: Matchup): string {
	return matchup.Home === 'BYE' ? matchup.Away : matchup.Home;
}
