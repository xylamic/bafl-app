<script lang="ts">
	import { onMount, tick } from 'svelte';
	import { loadEndpoint, peekCache, type Endpoint } from '$lib/api';
	import { formatLongDate, parseLocalDate } from '$lib/dates';
	import { safeUrl } from '$lib/links';
	import { createLoader } from '$lib/loader.svelte';
	import { pageTitle } from '$lib/pageTitle.svelte';
	import type { BaflEvent } from '$lib/types';
	import StatusBar from './StatusBar.svelte';
	import ThemedIcon from './ThemedIcon.svelte';

	interface Props {
		endpoint: Endpoint;
		/** Schedule group for the main tab: "Cheer" or "Drill". */
		mainGroup: string;
		fallbackTitle: string;
	}

	let { endpoint, mainGroup, fallbackTitle }: Props = $props();

	function toEvent(raw: unknown): BaflEvent {
		const e = raw as BaflEvent;
		if (!e || !Array.isArray(e.Schedule)) throw new Error('Unexpected event format');
		return e;
	}

	const loader = createLoader(
		() => loadEndpoint(endpoint, toEvent),
		() => peekCache(endpoint, toEvent)
	);

	let showMascot = $state(false);
	let firstLoad = true;
	let listEl: HTMLElement | undefined = $state();

	onMount(() => {
		void loader.refresh().then(() => {
			// Like iOS: open on the Mascot tab if that is where the live item is.
			if (firstLoad && loader.data?.Schedule.some((i) => i.Group === 'Mascot' && i.Highlight)) {
				showMascot = true;
			}
			firstLoad = false;
		});
		return loader.watchVisibility();
	});

	let event = $derived(loader.data);
	let items = $derived((event?.Schedule ?? []).filter((i) => i.Group === (showMascot ? 'Mascot' : mainGroup)));
	let hasHighlight = $derived(items.some((i) => i.Highlight));
	let moreInfo = $derived(safeUrl(event?.MoreInfo));
	let tickets = $derived(safeUrl(event?.Tickets));

	$effect(() => {
		pageTitle.value = event?.Name || fallbackTitle;
	});

	async function goToCurrent() {
		await tick();
		listEl?.querySelector('.highlight')?.scrollIntoView({ behavior: 'smooth', block: 'center' });
	}
</script>

<div class="page narrow">
	<StatusBar result={loader.result} loading={loader.loading} onrefresh={loader.refresh} />

	{#if event}
		{#if event.Message?.trim()}
			<p class="banner">{event.Message}</p>
		{/if}

		<p class="date">{formatLongDate(parseLocalDate(event.Date))}</p>
		{#if event.Information?.trim()}
			<p class="info">{event.Information}</p>
		{/if}

		<p class="meta">
			{#if event.DoorsOpen}<span>Doors open @ {event.DoorsOpen}</span>{/if}
			{#if moreInfo}<a href={moreInfo} target="_blank" rel="noopener noreferrer">Event Info</a>{/if}
			{#if tickets}<a href={tickets} target="_blank" rel="noopener noreferrer">Buy Tickets</a>{/if}
		</p>

		<div class="controls">
			<div class="segmented" role="group" aria-label="Schedule group">
				<button aria-pressed={!showMascot} onclick={() => (showMascot = false)}>{mainGroup}</button>
				<button aria-pressed={showMascot} onclick={() => (showMascot = true)}>Mascot</button>
			</div>
			<button class="icon-btn" aria-label="Go to the current performance" disabled={!hasHighlight} onclick={goToCurrent}>
				<ThemedIcon name="goto" size={28} />
			</button>
		</div>

		<table>
			<thead>
				<tr>
					<th scope="col">Team</th>
					<th scope="col">Scheduled</th>
					<th scope="col">Actual</th>
				</tr>
			</thead>
			<tbody bind:this={listEl}>
				{#each items as item, i (i)}
					<tr class:highlight={item.Highlight} class:notable={item.Notable}>
						<td>{item.Name}</td>
						<td>{item.ScheduledStart}</td>
						<td>{item.Status}</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
</div>

<style>
	.date {
		margin: 4px 0;
		text-align: center;
		font-size: 1.3rem;
		color: var(--secondary);
	}

	.info {
		margin: 4px 10px;
		text-align: center;
		font-size: 0.95rem;
	}

	.meta {
		display: flex;
		flex-wrap: wrap;
		justify-content: center;
		gap: 6px 20px;
		margin: 8px 0;
	}

	.controls {
		display: flex;
		align-items: center;
		gap: 8px;
		margin: 8px 0;
	}

	.controls .segmented {
		flex: 1;
	}

	table {
		width: 100%;
		border-collapse: separate;
		border-spacing: 0 4px;
	}

	th {
		font-size: 1.05rem;
		padding: 6px 4px;
		border-bottom: 4px solid var(--rule);
	}

	td {
		width: 33%;
		padding: 8px 6px;
		text-align: center;
		vertical-align: middle;
	}

	tr.highlight td {
		background: var(--highlight);
	}

	tr.notable td {
		border-top: 2px solid var(--gray-500);
		border-bottom: 2px solid var(--gray-500);
	}

	tr.notable td:first-child {
		border-left: 2px solid var(--gray-500);
		border-radius: 30px 0 0 30px;
	}

	tr.notable td:last-child {
		border-right: 2px solid var(--gray-500);
		border-radius: 0 30px 30px 0;
	}
</style>
