// Port of bafl-app/AgeWeightCalcView.xaml.cs. Keep the cutoff, ages, and weights in sync.

export const FOOTBALL_LEVELS = [
	'TOO YOUNG',
	'Peewee',
	'Freshman',
	'Sophomore',
	'Junior',
	'Senior',
	'AGED OUT'
] as const;

export const FOOTBALL_WEIGHTS = ['N/A', '130', '150', '170', '190', '210', 'N/A'] as const;

/** Reference table shown under the calculator. */
export const LEVEL_REFERENCE = [
	{ label: 'Peewee (5-6)', weight: FOOTBALL_WEIGHTS[1] },
	{ label: 'Freshman (7-8)', weight: FOOTBALL_WEIGHTS[2] },
	{ label: 'Sophomore (9)', weight: FOOTBALL_WEIGHTS[3] },
	{ label: 'Junior (10)', weight: FOOTBALL_WEIGHTS[4] },
	{ label: 'Senior (11-12)', weight: FOOTBALL_WEIGHTS[5] }
] as const;

export const WEIGHT_NOTE =
	'For football, the player must be at or under the maximum weight at the time of official monitoring. ' +
	'Once the season starts, one additional pound per week will be allowed leading up to Week 6. ' +
	'On Week 6, all players will be weighed again while wearing all equipment. ' +
	'They will be allowed an additional 10lbs for the equipment and 5lbs for the 5 weeks that had passed.';

/** Age is measured on August 1 of the current year. */
export function ageCutoffDate(now: Date = new Date()): Date {
	return new Date(now.getFullYear(), 7, 1);
}

/** Whole years from `birth` to `on`, counting a birthday on `on` as reached. */
export function ageOn(birth: Date, on: Date): number {
	let years = on.getFullYear() - birth.getFullYear();
	if (
		on.getMonth() < birth.getMonth() ||
		(on.getMonth() === birth.getMonth() && on.getDate() < birth.getDate())
	) {
		years--;
	}
	return years;
}

export interface LevelResult {
	age: number;
	level: string;
	weight: string;
}

export function calculateLevel(birth: Date, cutoff: Date, isCheer: boolean, moveUp: boolean): LevelResult {
	const age = ageOn(birth, cutoff);

	if (isCheer) {
		let level: string;
		if (age < 4) level = 'TOO YOUNG';
		else if (age < 8 && !moveUp) level = 'Mascot';
		else if (age < 14) level = 'Cheer/Drill';
		else level = 'AGED OUT';
		return { age, level, weight: 'N/A' };
	}

	let index: number;
	if (age < 5) index = 0;
	else if (age < 7) index = 1;
	else if (age < 9) index = 2;
	else if (age < 10) index = 3;
	else if (age < 11) index = 4;
	else if (age < 13) index = 5;
	else index = 6;

	if (moveUp && index > 0 && index < 5) index += 1;

	return { age, level: FOOTBALL_LEVELS[index] ?? 'N/A', weight: FOOTBALL_WEIGHTS[index] ?? 'N/A' };
}
