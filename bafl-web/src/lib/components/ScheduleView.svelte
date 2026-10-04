<script lang="ts">
	import { onMount } from 'svelte';
	import { goto } from '$app/navigation';
	import { page } from '$app/state';
	import { loadEndpoint, peekCache, type Endpoint } from '$lib/api';
	import { createLoader } from '$lib/loader.svelte';
	import { pageTitle } from '$lib/pageTitle.svelte';
	import { byeTeam, findClosestWeekIndex, isBye, matchupDivider, weekLabel } from '$lib/schedule';
	import type { GameCalendar } from '$lib/types';
	import StatusBar from './StatusBar.svelte';

	interface Props {
		endpoint: Endpoint;
		fallbackTitle: string;
	}

	let { endpoint, fallbackTitle }: Props = $props();

	function toCalendar(raw: unknown): GameCalendar {
		const c = raw as GameCalendar;
		if (!c || !Array.isArray(c.Weeks)) throw new Error('Unexpected schedule format');
		return c;
	}

	const loader = createLoader(
		() => loadEndpoint(endpoint, toCalendar),
		() => peekCache(endpoint, toCalendar)
	);

	onMount(() => {
		void loader.refresh();
		return loader.watchVisibility();
	});

	let calendar = $derived(loader.data);
	let weeks = $derived(calendar?.Weeks ?? []);

	let selectedIndex = $derived.by(() => {
		const param = Number(page.url.searchParams.get('week'));
		if (Number.isInteger(param) && param >= 1 && param <= weeks.length) return param - 1;
		return findClosestWeekIndex(weeks);
	});

	let week = $derived(weeks[selectedIndex]);
	let games = $derived((week?.Matchups ?? []).filter((m) => !isBye(m)));
	let byes = $derived((week?.Matchups ?? []).filter(isBye).map(byeTeam));

	$effect(() => {
		pageTitle.value = calendar?.Title || fallbackTitle;
	});

	function selectWeek(index: number) {
		if (index < 0 || index >= weeks.length) return;
		const url = new URL(page.url);
		url.searchParams.set('week', String(index + 1));
		void goto(url, { replaceState: true, keepFocus: true, noScroll: true });
	}
</script>

<div class="page">
	<StatusBar result={loader.result} loading={loader.loading} onrefresh={loader.refresh} />

	{#if loader.result?.notAvailable}
		<p class="banner">No {fallbackTitle.toLowerCase()} is published right now.</p>
	{:else if calendar}
		{#if calendar.Message?.trim()}
			<p class="banner">{calendar.Message}</p>
		{/if}

		{#if weeks.length > 0}
			<div class="picker">
				<button
					class="icon-btn"
					aria-label="Previous week"
					disabled={selectedIndex <= 0}
					onclick={() => selectWeek(selectedIndex - 1)}>‹</button
				>
				<label class="visually-hidden" for="week-select">Week</label>
				<select
					id="week-select"
					value={selectedIndex}
					onchange={(e) => selectWeek(Number(e.currentTarget.value))}
				>
					{#each weeks as w, i (i)}
						<option value={i}>{weekLabel(w)}</option>
					{/each}
				</select>
				<button
					class="icon-btn"
					aria-label="Next week"
					disabled={selectedIndex >= weeks.length - 1}
					onclick={() => selectWeek(selectedIndex + 1)}>›</button
				>
			</div>
			<hr class="rule thick" />
			<p class="legend muted">Scores are listed away @ home.</p>

			<div class="games">
				{#each games as m, i (i)}
					<article class="game">
						<div class="team away">{m.Away}</div>
						<div class="team home">
							<span class="divider">{matchupDivider(m)}</span>
							{m.Home}
						</div>
						{#if m.Scores?.length}
							<ul class="scores">
								{#each m.Scores as s, j (j)}
									<li>
										<span class="level">{s.Level}</span>
										<span class="score" class:tba={s.Score === 'TBA'}>{s.Score}</span>
									</li>
								{/each}
							</ul>
						{/if}
						{#if m.Details?.trim()}
							<p class="details">{m.Details}</p>
						{/if}
					</article>
				{/each}
			</div>

			{#if byes.length > 0}
				<p class="byes"><strong>BYE:</strong> {byes.join(', ')}</p>
			{/if}
		{:else}
			<p class="muted">No weeks have been published yet.</p>
		{/if}
	{/if}
</div>

<style>
	.picker {
		display: flex;
		align-items: center;
		justify-content: center;
		gap: 6px;
		margin: 8px 0;
	}

	.picker .icon-btn {
		font-size: 1.8rem;
		line-height: 1;
	}

	select {
		min-height: 44px;
		min-width: 0;
		max-width: 320px;
		flex: 1;
		padding: 0 0.6em;
		border: 1px solid var(--rule);
		border-radius: var(--radius);
		background: var(--bg);
		font-size: 1.05rem;
		text-align: center;
	}

	.legend {
		margin: 0 0 8px;
		text-align: center;
		font-size: 0.8rem;
	}

	.games {
		display: grid;
		grid-template-columns: 1fr;
		gap: 10px;
	}

	.game {
		padding: 10px 12px;
		border-bottom: 1px solid var(--rule);
		text-align: center;
	}

	.team {
		font-size: 1.2rem;
	}

	.divider {
		color: var(--secondary);
		margin-right: 0.3em;
	}

	.scores {
		display: flex;
		flex-wrap: wrap;
		justify-content: center;
		gap: 4px 14px;
		list-style: none;
		margin: 8px 0 0;
		padding: 0;
	}

	.level {
		color: var(--muted);
		font-size: 0.85rem;
		margin-right: 0.3em;
	}

	.score {
		font-variant-numeric: tabular-nums;
		white-space: nowrap;
	}

	.score.tba {
		color: var(--muted);
	}

	.details {
		margin: 6px 0 0;
		font-style: italic;
		font-size: 0.9rem;
	}

	.byes {
		text-align: center;
		margin-top: 14px;
		color: var(--muted);
	}

	/* iPad and desktop: matchups in a card grid. */
	@media (min-width: 700px) {
		.games {
			grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
			gap: 14px;
		}

		.game {
			border: 1px solid var(--surface);
			border-top: 3px solid var(--primary);
			border-radius: var(--radius);
			background: var(--surface);
		}
	}
</style>
