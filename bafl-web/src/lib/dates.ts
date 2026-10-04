const DATE_PATTERN = /^(\d{4})-(\d{2})-(\d{2})(?:T(\d{2}):(\d{2})(?::(\d{2}))?)?/;

/**
 * Parse an API date as local time, matching how .NET reads offset-less dates.
 * `new Date("2026-08-15")` would treat it as UTC midnight and shift the day in Houston.
 */
export function parseLocalDate(value: string | null | undefined): Date | null {
	if (!value) return null;
	const match = DATE_PATTERN.exec(value);
	if (!match) {
		const fallback = new Date(value);
		return Number.isNaN(fallback.getTime()) ? null : fallback;
	}
	const [, y, mo, d, h, mi, s] = match;
	return new Date(Number(y), Number(mo) - 1, Number(d), Number(h ?? 0), Number(mi ?? 0), Number(s ?? 0));
}

export function addDays(date: Date, days: number): Date {
	const result = new Date(date);
	result.setDate(result.getDate() + days);
	return result;
}

/** "Saturday, August 15, 2026" */
export function formatLongDate(date: Date | null): string {
	if (!date) return '';
	return date.toLocaleDateString('en-US', {
		weekday: 'long',
		month: 'long',
		day: 'numeric',
		year: 'numeric'
	});
}

/** "August 15" */
export function formatMonthDay(date: Date | null): string {
	if (!date) return '';
	return date.toLocaleDateString('en-US', { month: 'long', day: 'numeric' });
}
