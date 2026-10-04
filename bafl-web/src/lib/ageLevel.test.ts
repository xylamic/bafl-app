import { describe, expect, it } from 'vitest';
import { ageCutoffDate, ageOn, calculateLevel } from './ageLevel';

const cutoff = new Date(2026, 7, 1);
const born = (y: number, m: number, d: number) => new Date(y, m - 1, d);

describe('ageOn', () => {
	it('counts a birthday on the cutoff as reached', () => {
		expect(ageOn(born(2016, 8, 1), cutoff)).toBe(10);
		expect(ageOn(born(2016, 8, 2), cutoff)).toBe(9);
		expect(ageOn(born(2016, 7, 31), cutoff)).toBe(10);
	});

	it('handles a leap-day birthday', () => {
		expect(ageOn(born(2016, 2, 29), cutoff)).toBe(10);
	});

	it('uses August 1 of the current year', () => {
		expect(ageCutoffDate(new Date(2026, 9, 4))).toEqual(cutoff);
		expect(ageCutoffDate(new Date(2027, 0, 15))).toEqual(new Date(2027, 7, 1));
	});
});

describe('football levels', () => {
	const level = (age: number, moveUp = false) => calculateLevel(born(2026 - age, 8, 1), cutoff, false, moveUp);

	it.each([
		[4, 'TOO YOUNG', 'N/A'],
		[5, 'Peewee', '130'],
		[6, 'Peewee', '130'],
		[7, 'Freshman', '150'],
		[8, 'Freshman', '150'],
		[9, 'Sophomore', '170'],
		[10, 'Junior', '190'],
		[11, 'Senior', '210'],
		[12, 'Senior', '210'],
		[13, 'AGED OUT', 'N/A']
	])('age %i is %s at %s lbs', (age, name, weight) => {
		expect(level(age)).toEqual({ age, level: name, weight });
	});

	it('moves up one level from Peewee through Junior only', () => {
		expect(level(4, true).level).toBe('TOO YOUNG');
		expect(level(5, true)).toMatchObject({ level: 'Freshman', weight: '150' });
		expect(level(9, true)).toMatchObject({ level: 'Junior', weight: '190' });
		expect(level(10, true)).toMatchObject({ level: 'Senior', weight: '210' });
		expect(level(11, true).level).toBe('Senior');
		expect(level(13, true).level).toBe('AGED OUT');
	});
});

describe('cheer and drill levels', () => {
	const level = (age: number, moveUp = false) => calculateLevel(born(2026 - age, 8, 1), cutoff, true, moveUp);

	it('assigns cheer levels by age', () => {
		expect(level(3).level).toBe('TOO YOUNG');
		expect(level(4).level).toBe('Mascot');
		expect(level(7).level).toBe('Mascot');
		expect(level(8).level).toBe('Cheer/Drill');
		expect(level(13).level).toBe('Cheer/Drill');
		expect(level(14).level).toBe('AGED OUT');
		expect(level(8).weight).toBe('N/A');
	});

	it('moves a mascot-age child up to Cheer/Drill', () => {
		expect(level(5, true).level).toBe('Cheer/Drill');
		expect(level(3, true).level).toBe('TOO YOUNG');
	});
});
