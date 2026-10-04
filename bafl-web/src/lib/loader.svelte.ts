import type { LoadResult } from './api';

const STALE_AFTER_MS = 60_000;

/**
 * Reactive wrapper around an API load. Replaces pull-to-refresh with a refresh button
 * plus an automatic reload when the tab becomes visible again after a minute.
 */
export function createLoader<T>(
	load: () => Promise<LoadResult<T>>,
	initial: () => LoadResult<T> | null = () => null
) {
	const state = $state<{ result: LoadResult<T> | null; loading: boolean }>({ result: initial(), loading: false });
	let inFlight: Promise<void> | null = null;
	let lastAttempt = 0;

	function refresh(): Promise<void> {
		if (inFlight) return inFlight;
		state.loading = true;
		lastAttempt = Date.now();
		inFlight = load()
			.then((result) => {
				state.result = result;
			})
			.finally(() => {
				state.loading = false;
				inFlight = null;
			});
		return inFlight;
	}

	/** Call inside `$effect`; returns the cleanup. */
	function watchVisibility(): () => void {
		const onChange = () => {
			if (document.visibilityState === 'visible' && Date.now() - lastAttempt > STALE_AFTER_MS) {
				void refresh();
			}
		};
		document.addEventListener('visibilitychange', onChange);
		return () => document.removeEventListener('visibilitychange', onChange);
	}

	return {
		get result() {
			return state.result;
		},
		get data(): T | null {
			return state.result?.data ?? null;
		},
		get loading() {
			return state.loading;
		},
		refresh,
		watchVisibility
	};
}
