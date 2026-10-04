// Port of bafl-app/Library/BaflPlayerMonitor.cs and BaflTeamMonitor.cs.
// Rules and the export file format must stay identical so files move between iOS and web.

export const TOTAL_PLAYS_PEEWEE = 8;
export const TOTAL_PLAYS_FR_SR = 12;

/** Integer values match the C# `PlayerMissReasons` enum and the exported files. */
export const MISS_REASONS = ['NotSet', 'Injured', 'Absent', 'Discipline', 'Sick', 'Parent', 'Ejected'] as const;
export type MissReason = (typeof MISS_REASONS)[number];

export type PlayStatus = 'NoPlays' | 'PartialPlays' | 'CompletedPlays' | 'NotPlaying';

export interface PlayerMonitor {
	/** Runtime-only identity for undo; never exported. */
	id: number;
	isPeewee: boolean;
	number: number;
	name: string;
	/** Raw count; may exceed the target. Use `displayedPlays` for display. */
	plays: number;
	halfPlays: boolean;
	onField: boolean;
	isPlaying: boolean;
	notPlayReason: MissReason;
}

export interface TeamMonitor {
	isPeewee: boolean;
	players: PlayerMonitor[];
	thisTeam: string;
	opposingTeam: string;
	playCount: number;
	/** ISO 8601 timestamp of the last play run, or null. */
	lastPlay: string | null;
	/** Player ids that received the last play; runtime-only, like the C# undo list. */
	undo: number[];
}

let nextId = 1;

export function createPlayer(fields: Partial<Omit<PlayerMonitor, 'id'>> = {}): PlayerMonitor {
	return {
		id: nextId++,
		isPeewee: false,
		number: 0,
		name: '',
		plays: 0,
		halfPlays: false,
		onField: false,
		isPlaying: true,
		notPlayReason: 'NotSet',
		...fields
	};
}

export function createTeam(fields: Partial<Omit<TeamMonitor, 'undo'>> = {}): TeamMonitor {
	const team: TeamMonitor = {
		isPeewee: false,
		players: [],
		thisTeam: '',
		opposingTeam: '',
		playCount: 0,
		lastPlay: null,
		...fields,
		undo: []
	};
	if (!team.thisTeam.trim()) team.thisTeam = 'This team (edit)';
	if (!team.opposingTeam.trim()) team.opposingTeam = 'Opposing team (edit)';
	return team;
}

// ---------- Player rules ----------

export function playsTarget(player: PlayerMonitor): number {
	const target = player.isPeewee ? TOTAL_PLAYS_PEEWEE : TOTAL_PLAYS_FR_SR;
	return player.halfPlays ? Math.floor(target / 2) : target;
}

/** Plays capped at the target, as the C# `Plays` getter returns. */
export function displayedPlays(player: PlayerMonitor): number {
	return Math.min(player.plays, playsTarget(player));
}

export function playStatus(player: PlayerMonitor): PlayStatus {
	if (!player.isPlaying) return 'NotPlaying';
	if (player.plays <= 0) return 'NoPlays';
	return player.plays < playsTarget(player) ? 'PartialPlays' : 'CompletedPlays';
}

/** Same text as the iOS "Left" column. */
export function playsLabel(player: PlayerMonitor): string {
	if (!player.isPlaying) {
		return player.notPlayReason === 'NotSet' ? 'N/A' : player.notPlayReason;
	}
	const target = playsTarget(player);
	const plays = displayedPlays(player);
	if (plays >= target) return '✅';
	const left = target - plays;
	return `${left} ${left > Math.floor(target / 2) ? '🟥' : '🟨'}`;
}

export function addPlay(player: PlayerMonitor): boolean {
	if (player.isPlaying && player.onField) {
		player.plays += 1;
		return true;
	}
	return false;
}

export function removePlay(player: PlayerMonitor): void {
	if (displayedPlays(player) > 0) player.plays -= 1;
}

export function setOnField(player: PlayerMonitor, onField: boolean): void {
	if (player.isPlaying) player.onField = onField;
}

/** Matches iOS: `onField` is kept when a player is pulled, so reinstating returns them to the field. */
export function setPlaying(player: PlayerMonitor, isPlaying: boolean): void {
	player.isPlaying = isPlaying;
}

export function resetPlayer(player: PlayerMonitor): void {
	player.plays = 0;
	player.halfPlays = false;
}

// ---------- Team rules ----------

export function playersOnField(team: TeamMonitor): number {
	return team.players.filter((p) => p.onField && p.isPlaying).length;
}

export function undoAllowed(team: TeamMonitor): boolean {
	return team.undo.length > 0;
}

export function teamComplete(team: TeamMonitor): boolean {
	return team.players.every((p) => {
		const status = playStatus(p);
		return status !== 'NoPlays' && status !== 'PartialPlays';
	});
}

export function setTeamPeewee(team: TeamMonitor, isPeewee: boolean): void {
	team.isPeewee = isPeewee;
	for (const p of team.players) p.isPeewee = isPeewee;
}

export function runPlay(team: TeamMonitor, now: Date = new Date()): void {
	team.undo = [];
	for (const p of team.players) {
		if (addPlay(p)) team.undo.push(p.id);
	}
	if (team.undo.length > 0) {
		team.playCount += 1;
		team.lastPlay = toLocalIsoString(now);
	}
}

export function undoPlay(team: TeamMonitor): void {
	if (team.undo.length === 0) return;
	for (const id of team.undo) {
		const player = team.players.find((p) => p.id === id);
		if (player) removePlay(player);
	}
	team.playCount -= 1;
	team.undo = [];
}

/** Stable sort by jersey number, like LINQ `OrderBy`. */
export function sortPlayers(team: TeamMonitor): void {
	team.players = [...team.players].sort((a, b) => a.number - b.number);
}

export function setAllOnField(team: TeamMonitor, onField: boolean): void {
	for (const p of team.players) setOnField(p, onField);
}

export function resetPlays(team: TeamMonitor): void {
	for (const p of team.players) resetPlayer(p);
	team.undo = [];
	team.lastPlay = null;
	team.playCount = 0;
}

/** Same defaults as the iOS "+" button. */
export function addPlayer(team: TeamMonitor): PlayerMonitor {
	const player = createPlayer({ isPeewee: team.isPeewee, number: 99, name: '-' });
	team.players.push(player);
	return player;
}

export function deletePlayer(team: TeamMonitor, id: number): void {
	team.players = team.players.filter((p) => p.id !== id);
}

// ---------- Import / export (SBaflTeamMonitor / SBaflPlayerMonitor) ----------

interface SPlayer {
	IsPeewee: boolean;
	Number: number;
	Name: string;
	Plays: number;
	HalfPlays: boolean;
	OnField: boolean;
	IsPlaying: boolean;
	NotPlayReason: number;
}

interface STeam {
	IsPeewee: boolean;
	Players: string[];
	ThisTeam: string;
	OpposingTeam: string;
	PlayCount: number;
	LastPlay: string | null;
}

export function exportTeamJson(team: TeamMonitor): string {
	// Property order matches System.Text.Json output from the iOS app.
	const data: STeam = {
		IsPeewee: team.isPeewee,
		Players: team.players.map((p) => {
			const sp: SPlayer = {
				IsPeewee: p.isPeewee,
				Number: p.number,
				Name: p.name,
				Plays: displayedPlays(p),
				HalfPlays: p.halfPlays,
				OnField: p.onField,
				IsPlaying: p.isPlaying,
				NotPlayReason: MISS_REASONS.indexOf(p.notPlayReason)
			};
			return JSON.stringify(sp);
		}),
		ThisTeam: team.thisTeam,
		OpposingTeam: team.opposingTeam,
		PlayCount: team.playCount,
		LastPlay: team.lastPlay
	};
	return JSON.stringify(data);
}

function asBool(value: unknown, field: string): boolean {
	if (typeof value !== 'boolean') throw new Error(`Invalid ${field}`);
	return value;
}

function asInt(value: unknown, field: string): number {
	if (typeof value !== 'number' || !Number.isInteger(value)) throw new Error(`Invalid ${field}`);
	return value;
}

function asString(value: unknown, field: string): string {
	if (value === null || value === undefined) return '';
	if (typeof value !== 'string') throw new Error(`Invalid ${field}`);
	return value;
}

function importPlayer(value: unknown): PlayerMonitor {
	const sp = (typeof value === 'string' ? JSON.parse(value) : value) as Record<string, unknown>;
	if (!sp || typeof sp !== 'object') throw new Error('Invalid player');
	const reasonIndex = asInt(sp.NotPlayReason ?? 0, 'NotPlayReason');
	return createPlayer({
		isPeewee: asBool(sp.IsPeewee, 'IsPeewee'),
		number: asInt(sp.Number, 'Number'),
		name: asString(sp.Name, 'Name'),
		plays: Math.max(0, asInt(sp.Plays, 'Plays')),
		halfPlays: asBool(sp.HalfPlays, 'HalfPlays'),
		onField: asBool(sp.OnField, 'OnField'),
		isPlaying: asBool(sp.IsPlaying, 'IsPlaying'),
		notPlayReason: MISS_REASONS[reasonIndex] ?? 'NotSet'
	});
}

/** Throws if the JSON is not a valid team monitor file. */
export function importTeamJson(json: string): TeamMonitor {
	const data = JSON.parse(json) as Record<string, unknown>;
	if (!data || typeof data !== 'object' || !Array.isArray(data.Players)) {
		throw new Error('Not a team monitor file');
	}
	const lastPlay = data.LastPlay;
	if (lastPlay !== null && lastPlay !== undefined && typeof lastPlay !== 'string') {
		throw new Error('Invalid LastPlay');
	}
	return createTeam({
		isPeewee: asBool(data.IsPeewee, 'IsPeewee'),
		players: data.Players.map(importPlayer),
		thisTeam: asString(data.ThisTeam, 'ThisTeam'),
		opposingTeam: asString(data.OpposingTeam, 'OpposingTeam'),
		playCount: asInt(data.PlayCount, 'PlayCount'),
		lastPlay: lastPlay ?? null
	});
}

// ---------- Formatting ----------

function pad(n: number, width = 2): string {
	return String(Math.abs(n)).padStart(width, '0');
}

/** ISO 8601 with the local offset, like .NET serializes a local `DateTime`. */
export function toLocalIsoString(date: Date): string {
	const offset = -date.getTimezoneOffset();
	const sign = offset >= 0 ? '+' : '-';
	return (
		`${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}` +
		`T${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}.${pad(date.getMilliseconds(), 3)}` +
		`${sign}${pad(Math.floor(Math.abs(offset) / 60))}:${pad(Math.abs(offset) % 60)}`
	);
}

/** "2026-10-4 3:05 PM", the iOS `yyyy-M-d h:mm tt` format. */
export function formatLastPlay(lastPlay: string | null): string {
	if (!lastPlay) return 'No plays';
	// .NET writes 7 fractional digits; trim to 3 so every browser's Date parser accepts it.
	const date = new Date(lastPlay.replace(/(\.\d{3})\d+/, '$1'));
	if (Number.isNaN(date.getTime())) return 'No plays';
	const hours = date.getHours();
	const h12 = hours % 12 === 0 ? 12 : hours % 12;
	return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()} ${h12}:${pad(date.getMinutes())} ${hours < 12 ? 'AM' : 'PM'}`;
}
