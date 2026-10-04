<script lang="ts">
	import type { LoadResult } from '$lib/api';

	interface Props {
		result: LoadResult<unknown> | null;
		loading: boolean;
		onrefresh: () => void;
	}

	let { result, loading, onrefresh }: Props = $props();

	const timeFormat: Intl.DateTimeFormatOptions = { hour: 'numeric', minute: '2-digit' };
	const dateTimeFormat: Intl.DateTimeFormatOptions = { month: 'short', day: 'numeric', ...timeFormat };

	let text = $derived.by(() => {
		if (!result) return loading ? 'Loading…' : '';
		if (loading) return 'Updating…';
		const at = result.updatedAt;
		switch (result.source) {
			case 'network':
				return `Updated ${at?.toLocaleTimeString('en-US', timeFormat) ?? ''}`;
			case 'cache':
				return `Offline: showing data saved ${at?.toLocaleString('en-US', dateTimeFormat) ?? ''}`;
			case 'bundled':
				return 'Offline: showing built-in data';
			default:
				return result.notAvailable ? '' : `Could not load (${result.error ?? 'unknown error'})`;
		}
	});

	let isProblem = $derived(result !== null && !loading && result.source !== 'network' && !result.notAvailable);
</script>

<div class="status" class:problem={isProblem} role="status">
	<span>{text}</span>
	<button class="btn secondary refresh" onclick={onrefresh} disabled={loading} aria-label="Refresh">
		<span class:spin={loading} aria-hidden="true">↻</span>
		<span class="label">{loading ? 'Loading' : 'Refresh'}</span>
	</button>
</div>

<style>
	.status {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: 8px;
		min-height: 44px;
		font-size: 0.85rem;
		color: var(--muted);
		border-bottom: 1px solid var(--rule);
		padding-bottom: 6px;
		margin-bottom: 6px;
	}

	.status.problem span:first-child {
		color: var(--warn);
	}

	.refresh {
		min-height: 34px;
		padding: 0.2em 0.8em;
		font-size: 0.9rem;
	}

	.spin {
		display: inline-block;
		animation: spin 1s linear infinite;
	}

	@keyframes spin {
		to {
			transform: rotate(360deg);
		}
	}
</style>
