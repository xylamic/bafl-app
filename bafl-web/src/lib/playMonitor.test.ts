// Ported from bafl-app/Testing/BaflPlayMonitorTest.cs, plus iOS file-format compatibility checks.
import { describe, expect, it } from 'vitest';
import {
	addPlay,
	addPlayer,
	createPlayer,
	createTeam,
	deletePlayer,
	displayedPlays,
	exportTeamJson,
	formatLastPlay,
	importTeamJson,
	playersOnField,
	playsLabel,
	playStatus,
	removePlay,
	resetPlays,
	runPlay,
	setAllOnField,
	setOnField,
	setPlaying,
	setTeamPeewee,
	sortPlayers,
	teamComplete,
	toLocalIsoString,
	undoAllowed,
	undoPlay,
	type PlayerMonitor,
	type TeamMonitor
} from './playMonitor';

describe('player monitor', () => {
	it('starts empty (EmptyPlayerMonitor)', () => {
		const p = createPlayer();
		expect(p.isPeewee).toBe(false);
		expect(p.name).toBe('');
		expect(p.number).toBe(0);
		expect(displayedPlays(p)).toBe(0);
		expect(playStatus(p)).toBe('NoPlays');
		expect(p.halfPlays).toBe(false);
		expect(p.onField).toBe(false);
		expect(p.isPlaying).toBe(true);
	});

	it('reports partial plays for a half-play player (SimplePlayerMonitor/2)', () => {
		const p = createPlayer({ number: 12, name: 'John Doe', plays: 5, halfPlays: true, onField: true });
		expect(displayedPlays(p)).toBe(5);
		expect(playStatus(p)).toBe('PartialPlays');
	});

	it('caps a peewee half-play player at 4 (SimplePlayerMonitor3)', () => {
		const p = createPlayer({ isPeewee: true, number: 12, plays: 5, halfPlays: true });
		expect(displayedPlays(p)).toBe(4);
		expect(playStatus(p)).toBe('CompletedPlays');
	});

	it('peewee full player at 5 of 8 is partial (SimplePlayerMonitor4)', () => {
		const p = createPlayer({ isPeewee: true, number: 12, plays: 5 });
		expect(displayedPlays(p)).toBe(5);
		expect(playStatus(p)).toBe('PartialPlays');
	});

	it('reports NotPlaying when inactive (NotPlayingPlayerMonitor)', () => {
		const p = createPlayer({ isPeewee: true, number: 12, isPlaying: false });
		expect(playStatus(p)).toBe('NotPlaying');
		expect(displayedPlays(p)).toBe(0);
	});

	it('counts plays up to 12 and back (PlayerMonitorPlayCount)', () => {
		const p = createPlayer({ number: 12, onField: true });
		for (let i = 0; i < 5; i++) addPlay(p);
		expect(displayedPlays(p)).toBe(5);
		expect(playStatus(p)).toBe('PartialPlays');

		for (let i = 0; i < 5; i++) addPlay(p);
		expect(displayedPlays(p)).toBe(10);

		for (let i = 0; i < 5; i++) addPlay(p);
		expect(displayedPlays(p)).toBe(12);
		expect(playStatus(p)).toBe('CompletedPlays');

		for (let i = 0; i < 5; i++) removePlay(p);
		expect(displayedPlays(p)).toBe(10);
		expect(playStatus(p)).toBe('PartialPlays');

		addPlay(p);
		addPlay(p);
		expect(displayedPlays(p)).toBe(12);
		expect(playStatus(p)).toBe('CompletedPlays');
	});

	it('targets 6 plays for a half-play player (PlayerMonitorPlayCountHalf)', () => {
		const p = createPlayer({ number: 15, onField: true });
		p.halfPlays = true;
		for (let i = 0; i < 5; i++) addPlay(p);
		expect(displayedPlays(p)).toBe(5);
		expect(playStatus(p)).toBe('PartialPlays');
		addPlay(p);
		expect(displayedPlays(p)).toBe(6);
		expect(playStatus(p)).toBe('CompletedPlays');
		removePlay(p);
		expect(displayedPlays(p)).toBe(5);
		addPlay(p);
		expect(playStatus(p)).toBe('CompletedPlays');
	});

	it('does not add plays when off the field or not playing', () => {
		const off = createPlayer({ onField: false });
		expect(addPlay(off)).toBe(false);
		const out = createPlayer({ onField: true, isPlaying: false });
		expect(addPlay(out)).toBe(false);
	});

	it('ignores on-field changes for inactive players', () => {
		const p = createPlayer({ isPlaying: false });
		setOnField(p, true);
		expect(p.onField).toBe(false);
	});

	it('keeps onField when pulled, like iOS', () => {
		const p = createPlayer({ onField: true });
		setPlaying(p, false);
		expect(p.onField).toBe(true);
		setPlaying(p, true);
		expect(p.onField).toBe(true);
	});

	it('labels plays left like the iOS "Left" column', () => {
		expect(playsLabel(createPlayer({ plays: 0 }))).toBe('12 🟥');
		expect(playsLabel(createPlayer({ plays: 6 }))).toBe('6 🟨');
		expect(playsLabel(createPlayer({ plays: 5 }))).toBe('7 🟥');
		expect(playsLabel(createPlayer({ plays: 12 }))).toBe('✅');
		expect(playsLabel(createPlayer({ isPlaying: false }))).toBe('N/A');
		expect(playsLabel(createPlayer({ isPlaying: false, notPlayReason: 'Injured' }))).toBe('Injured');
	});
});

describe('team monitor', () => {
	it('fills placeholder team names (C# CheckTeamNames)', () => {
		// The C# EmptyTeamMonitor test expects "", but CheckTeamNames sets these placeholders.
		const team = createTeam();
		expect(team.thisTeam).toBe('This team (edit)');
		expect(team.opposingTeam).toBe('Opposing team (edit)');
		expect(team.playCount).toBe(0);
		expect(team.players).toHaveLength(0);
	});

	it('runs, caps, and undoes plays across a game (CompleteTeamMonitor)', () => {
		const players: PlayerMonitor[] = [];
		for (let i = 0; i < 5; i++) players.push(createPlayer({ number: i, name: `Player ${i}`, onField: true }));
		for (let i = 5; i < 8; i++) players.push(createPlayer({ number: i, name: `Player ${i}` }));
		players.push(createPlayer({ number: 8, name: 'Player 8', isPlaying: false }));

		const team = createTeam({ players, thisTeam: 'Bay Area', opposingTeam: 'Hitchcock' });
		expect(team.thisTeam).toBe('Bay Area');
		expect(team.players).toHaveLength(9);

		runPlay(team);
		runPlay(team);
		expect(team.players.map(displayedPlays)).toEqual([2, 2, 2, 2, 2, 0, 0, 0, 0]);
		expect(teamComplete(team)).toBe(false);

		setOnField(team.players[4]!, false);
		setOnField(team.players[5]!, true);
		setOnField(team.players[6]!, true);
		runPlay(team);
		runPlay(team);
		expect(team.players.map(displayedPlays)).toEqual([4, 4, 4, 4, 2, 2, 2, 0, 0]);

		for (let i = 0; i < 9; i++) runPlay(team);
		expect(team.players.map(displayedPlays)).toEqual([12, 12, 12, 12, 2, 11, 11, 0, 0]);
		expect(team.players.map(playStatus)).toEqual([
			'CompletedPlays',
			'CompletedPlays',
			'CompletedPlays',
			'CompletedPlays',
			'PartialPlays',
			'PartialPlays',
			'PartialPlays',
			'NoPlays',
			'NotPlaying'
		]);

		const onField = [false, false, false, true, true, true, true, true, false];
		team.players.forEach((p, i) => setOnField(p, onField[i]!));
		for (let i = 0; i < 12; i++) runPlay(team);
		expect(team.players.map((p) => p.plays)).toEqual([13, 13, 13, 25, 14, 23, 23, 12, 0]);
		expect(teamComplete(team)).toBe(true);
		expect(team.playCount).toBe(25);

		undoPlay(team);
		expect(teamComplete(team)).toBe(false);
		expect(team.players.map(displayedPlays)).toEqual([12, 12, 12, 12, 12, 12, 12, 11, 0]);
		expect(team.playCount).toBe(24);
		expect(undoAllowed(team)).toBe(false);

		runPlay(team);
		expect(teamComplete(team)).toBe(true);
		expect(team.playCount).toBe(25);
	});

	it('does not count a play when nobody is on the field', () => {
		const team = createTeam({ players: [createPlayer()] });
		runPlay(team);
		expect(team.playCount).toBe(0);
		expect(team.lastPlay).toBeNull();
		expect(undoAllowed(team)).toBe(false);
	});

	it('undo follows players after a sort', () => {
		const a = createPlayer({ number: 20, onField: true });
		const b = createPlayer({ number: 3 });
		const team = createTeam({ players: [a, b] });
		runPlay(team);
		sortPlayers(team);
		expect(team.players.map((p) => p.number)).toEqual([3, 20]);
		undoPlay(team);
		expect(a.plays).toBe(0);
		expect(b.plays).toBe(0);
	});

	it('still decrements the play count if an undone player was deleted', () => {
		const a = createPlayer({ onField: true });
		const team = createTeam({ players: [a] });
		runPlay(team);
		deletePlayer(team, a.id);
		undoPlay(team);
		expect(team.playCount).toBe(0);
	});

	it('sorts stably by number', () => {
		const team = createTeam({
			players: [
				createPlayer({ number: 5, name: 'a' }),
				createPlayer({ number: 1, name: 'b' }),
				createPlayer({ number: 5, name: 'c' })
			]
		});
		sortPlayers(team);
		expect(team.players.map((p) => p.name)).toEqual(['b', 'a', 'c']);
	});

	it('counts on-field players who are playing', () => {
		const team = createTeam({
			players: [createPlayer({ onField: true }), createPlayer({ onField: true, isPlaying: false }), createPlayer()]
		});
		expect(playersOnField(team)).toBe(1);
		setAllOnField(team, true);
		expect(playersOnField(team)).toBe(2);
	});

	it('resets plays, half plays, and the last play time', () => {
		const team = createTeam({ players: [createPlayer({ onField: true, halfPlays: true })] });
		runPlay(team);
		resetPlays(team);
		expect(team.playCount).toBe(0);
		expect(team.lastPlay).toBeNull();
		expect(team.players[0]!.plays).toBe(0);
		expect(team.players[0]!.halfPlays).toBe(false);
	});

	it('adds players with the iOS defaults and propagates peewee', () => {
		const team = createTeam({ isPeewee: true });
		const p = addPlayer(team);
		expect(p).toMatchObject({ number: 99, name: '-', plays: 0, onField: false, isPlaying: true, isPeewee: true });
		setTeamPeewee(team, false);
		expect(p.isPeewee).toBe(false);
	});
});

describe('file format', () => {
	// Shape produced by System.Text.Json in the iOS app: Players is a list of JSON strings.
	const iosFile = JSON.stringify({
		IsPeewee: false,
		Players: [
			JSON.stringify({
				IsPeewee: false,
				Number: 7,
				Name: 'Ava',
				Plays: 12,
				HalfPlays: false,
				OnField: true,
				IsPlaying: true,
				NotPlayReason: 0
			}),
			JSON.stringify({
				IsPeewee: false,
				Number: 22,
				Name: 'Ben',
				Plays: 3,
				HalfPlays: true,
				OnField: false,
				IsPlaying: false,
				NotPlayReason: 1
			})
		],
		ThisTeam: 'Bay Area Buccaneers',
		OpposingTeam: 'Hitchcock Red Raiders',
		PlayCount: 14,
		LastPlay: '2026-09-26T10:41:07.1234567-05:00'
	});

	it('imports an iOS export', () => {
		const team = importTeamJson(iosFile);
		expect(team.thisTeam).toBe('Bay Area Buccaneers');
		expect(team.opposingTeam).toBe('Hitchcock Red Raiders');
		expect(team.playCount).toBe(14);
		expect(team.players).toHaveLength(2);
		expect(team.players[0]).toMatchObject({ number: 7, name: 'Ava', plays: 12, onField: true, isPlaying: true });
		expect(team.players[1]).toMatchObject({ number: 22, halfPlays: true, isPlaying: false, notPlayReason: 'Injured' });
		expect(team.lastPlay).toBe('2026-09-26T10:41:07.1234567-05:00');
	});

	it('round-trips to the same JSON the iOS app writes', () => {
		expect(exportTeamJson(importTeamJson(iosFile))).toBe(iosFile);
	});

	it('exports capped plays, matching the C# Plays getter', () => {
		const team = createTeam({ players: [createPlayer({ plays: 15 })] });
		const exported = JSON.parse(exportTeamJson(team));
		expect(JSON.parse(exported.Players[0]).Plays).toBe(12);
	});

	it('rejects files that are not team monitors', () => {
		expect(() => importTeamJson('{}')).toThrow();
		expect(() => importTeamJson('not json')).toThrow();
		expect(() => importTeamJson(JSON.stringify({ IsPeewee: false, Players: ['{"Number":"x"}'] }))).toThrow();
	});

	it('maps unknown miss reasons to NotSet', () => {
		const file = JSON.stringify({
			IsPeewee: false,
			Players: [
				JSON.stringify({
					IsPeewee: false,
					Number: 1,
					Name: 'x',
					Plays: 0,
					HalfPlays: false,
					OnField: false,
					IsPlaying: false,
					NotPlayReason: 42
				})
			],
			ThisTeam: '',
			OpposingTeam: '',
			PlayCount: 0,
			LastPlay: null
		});
		const team: TeamMonitor = importTeamJson(file);
		expect(team.players[0]!.notPlayReason).toBe('NotSet');
		expect(team.thisTeam).toBe('This team (edit)');
	});

	it('writes local ISO timestamps that the iOS app can parse', () => {
		const iso = toLocalIsoString(new Date(2026, 9, 4, 15, 5, 9, 42));
		expect(iso).toMatch(/^2026-10-04T15:05:09\.042[+-]\d{2}:\d{2}$/);
		expect(new Date(iso).getTime()).toBe(new Date(2026, 9, 4, 15, 5, 9, 42).getTime());
	});

	it('formats the last play like iOS', () => {
		expect(formatLastPlay(null)).toBe('No plays');
		expect(formatLastPlay(toLocalIsoString(new Date(2026, 9, 4, 15, 5)))).toBe('2026-10-4 3:05 PM');
		expect(formatLastPlay(toLocalIsoString(new Date(2026, 9, 4, 0, 30)))).toBe('2026-10-4 12:30 AM');
	});
});
