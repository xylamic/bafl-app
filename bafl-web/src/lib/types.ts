// Field names mirror the JSON from the BAFL API exactly (case-sensitive).
// Specs: docs/game_schedule.md, docs/football_standings.md, docs/azure-data-integration.md.
// C# counterparts: bafl-app/Library/*.cs.

export const LEVELS = ['Peewee', 'Freshman', 'Sophomore', 'Junior', 'Senior'] as const;
export type Level = (typeof LEVELS)[number];

export interface AppConfig {
	Key: string;
}

export interface Club {
	Region: string;
	Football: string;
	Cheer: string;
	Mascot: string;
	Website: string;
	President: string;
	FieldName: string;
	FieldLocation: string;
}

/** Keyed by integer club ID (as a JSON string key). */
export type ClubMap = Record<string, Club>;

export interface BoardMember {
	Role: string;
	Name: string;
	Email: string;
}

export interface ScheduleItem {
	Date: string;
	Name: string;
	Location: string;
	Notable?: boolean;
}

export interface CoreInfo {
	teams: ClubMap;
	board: BoardMember[];
	schedule: ScheduleItem[];
}

export interface MatchupScore {
	Level: string;
	/** `"<away> @ <home>"`, `"TBA"`, or postseason text. */
	Score: string;
}

export interface Matchup {
	Home: string;
	Away: string;
	IsNeutral?: boolean;
	Details?: string;
	Scores?: MatchupScore[];
}

export interface GameWeek {
	Week: string;
	Date: string;
	Matchups: Matchup[];
}

export interface GameCalendar {
	Title: string;
	Message: string;
	Weeks: GameWeek[];
}

export interface StandingTeam {
	Team: string;
	Wins: number;
	Losses: number;
	Ties: number;
	/** Informational only; the app derives points from Wins and Ties. */
	Points?: number;
	Playoff: boolean;
	Rank: number;
}

export interface StandingEntry {
	Level: string;
	Teams: StandingTeam[];
}

export interface Standings {
	Title: string;
	Message: string;
	Standings: StandingEntry[];
}

export interface EventLineItem {
	Group: string;
	Name: string;
	ScheduledStart: string;
	Status: string;
	Highlight: boolean;
	Notable: boolean;
}

export interface BaflEvent {
	Name: string;
	Date: string;
	Message: string;
	Information: string;
	DoorsOpen: string;
	MoreInfo: string;
	Tickets: string;
	Schedule: EventLineItem[];
}
