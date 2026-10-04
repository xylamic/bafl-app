import type { AppConfig, BoardMember, ClubMap, CoreInfo, ScheduleItem } from './types';

// Keep in sync with bafl-app/Library/BaflUtilities.cs and docs/azure-data-integration.md.
const PRODUCTION_API_BASE = 'https://baflapp.azurewebsites.net/api';

export const API_BASE: string =
	import.meta.env.VITE_API_BASE ?? (import.meta.env.DEV ? '/api' : PRODUCTION_API_BASE);

export type Endpoint = 'coreinfo' | 'calendar' | 'standings' | 'cheercomp' | 'drillcomp';

export type DataSource = 'network' | 'cache' | 'bundled' | 'none';

export interface LoadResult<T> {
	data: T | null;
	source: DataSource;
	/** When the shown data was fetched from the network. */
	updatedAt: Date | null;
	error: string | null;
	/** The endpoint answered 404 (nothing published). */
	notAvailable: boolean;
}

class HttpError extends Error {
	constructor(readonly status: number) {
		super(`HTTP ${status}`);
	}
}

const CACHE_PREFIX = 'bafl-cache:';

let keyPromise: Promise<string> | null = null;

async function requestKey(): Promise<string> {
	// A plain string body is sent as text/plain, which keeps this a CORS "simple request" (no preflight).
	const response = await fetch(`${API_BASE}/app-config`, {
		method: 'POST',
		body: JSON.stringify({ context: 'BaflApp' })
	});
	if (!response.ok) throw new HttpError(response.status);
	const config = (await response.json()) as AppConfig;
	if (!config?.Key) throw new Error('No access key returned');
	return config.Key;
}

function getKey(forceRefresh = false): Promise<string> {
	if (!keyPromise || forceRefresh) {
		keyPromise = requestKey().catch((err) => {
			keyPromise = null;
			throw err;
		});
	}
	return keyPromise;
}

async function fetchEndpoint(endpoint: Endpoint, key: string): Promise<unknown> {
	const response = await fetch(`${API_BASE}/${endpoint}?code=${encodeURIComponent(key)}`);
	if (!response.ok) throw new HttpError(response.status);
	return response.json();
}

async function fetchJson(endpoint: Endpoint): Promise<unknown> {
	try {
		return await fetchEndpoint(endpoint, await getKey());
	} catch (err) {
		if (err instanceof HttpError && err.status === 404) throw err;
		// The key may have rotated; per docs/azure-data-integration.md, refresh it and retry once.
		return fetchEndpoint(endpoint, await getKey(true));
	}
}

interface CacheEntry {
	savedAt: string;
	data: unknown;
}

function readCache(endpoint: Endpoint): CacheEntry | null {
	if (typeof localStorage === 'undefined') return null;
	try {
		const raw = localStorage.getItem(CACHE_PREFIX + endpoint);
		return raw ? (JSON.parse(raw) as CacheEntry) : null;
	} catch {
		return null;
	}
}

function writeCache(endpoint: Endpoint, data: unknown, savedAt: Date): void {
	if (typeof localStorage === 'undefined') return;
	try {
		const entry: CacheEntry = { savedAt: savedAt.toISOString(), data };
		localStorage.setItem(CACHE_PREFIX + endpoint, JSON.stringify(entry));
	} catch {
		// Storage full or blocked (private mode); the app still works without the cache.
	}
}

function describeError(err: unknown): string {
	if (err instanceof HttpError) return err.message;
	if (err instanceof TypeError) return 'Network unavailable';
	return err instanceof Error ? err.message : String(err);
}

/** The last good copy from localStorage, shown instantly while a fresh load runs. */
export function peekCache<T>(endpoint: Endpoint, transform: (raw: unknown) => T = (raw) => raw as T): LoadResult<T> | null {
	const cached = readCache(endpoint);
	if (!cached) return null;
	try {
		return {
			data: transform(cached.data),
			source: 'cache',
			updatedAt: new Date(cached.savedAt),
			error: null,
			notAvailable: false
		};
	} catch {
		return null;
	}
}

/**
 * Fetch an endpoint, falling back to the last good copy in localStorage.
 * `transform` converts the raw JSON into the shape the page uses.
 */
export async function loadEndpoint<T>(
	endpoint: Endpoint,
	transform: (raw: unknown) => T = (raw) => raw as T
): Promise<LoadResult<T>> {
	try {
		const raw = await fetchJson(endpoint);
		const data = transform(raw);
		const now = new Date();
		writeCache(endpoint, raw, now);
		return { data, source: 'network', updatedAt: now, error: null, notAvailable: false };
	} catch (err) {
		const notAvailable = err instanceof HttpError && err.status === 404;
		const cached = notAvailable ? null : readCache(endpoint);
		if (cached) {
			try {
				return {
					data: transform(cached.data),
					source: 'cache',
					updatedAt: new Date(cached.savedAt),
					error: describeError(err),
					notAvailable: false
				};
			} catch {
				// Corrupt cache entry; fall through to "no data".
			}
		}
		return { data: null, source: 'none', updatedAt: null, error: describeError(err), notAvailable };
	}
}

/** `/api/coreinfo` returns each value as a JSON-encoded string that needs a second parse. */
export function parseCoreInfo(raw: unknown): CoreInfo {
	const obj = raw as Record<string, unknown>;
	const parse = <T>(value: unknown): T => (typeof value === 'string' ? JSON.parse(value) : value) as T;
	return {
		teams: parse<ClubMap>(obj.teams) ?? {},
		board: parse<BoardMember[]>(obj.board) ?? [],
		schedule: parse<ScheduleItem[]>(obj.schedule) ?? []
	};
}

/** Core info with a final fallback to the copies bundled in static/data. */
export async function loadCoreInfo(fetchImpl: typeof fetch = fetch): Promise<LoadResult<CoreInfo>> {
	const result = await loadEndpoint('coreinfo', parseCoreInfo);
	if (result.data) return result;

	try {
		const [teams, board, schedule] = await Promise.all(
			['Teams', 'Board', 'Schedule'].map((name) =>
				fetchImpl(`/data/${name}.json`).then((r) => {
					if (!r.ok) throw new HttpError(r.status);
					return r.json();
				})
			)
		);
		return {
			...result,
			data: { teams: teams as ClubMap, board: board as BoardMember[], schedule: schedule as ScheduleItem[] },
			source: 'bundled'
		};
	} catch {
		return result;
	}
}
