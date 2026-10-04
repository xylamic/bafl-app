<script lang="ts">
	import { onMount } from 'svelte';
	import { goto } from '$app/navigation';
	import { page } from '$app/state';
	import { loadEndpoint, peekCache, type Endpoint } from '$lib/api';
	import { createLoader } from '$lib/loader.svelte';
	import { pageTitle } from '$lib/pageTitle.svelte';
	import { sortStandingTeams, teamPoints } from '$lib/standings';
	import type { Standings } from '$lib/types';
	import StatusBar from './StatusBar.svelte';

	interface Props {
		endpoint: Endpoint;
		fallbackTitle: string;
	}

	let { endpoint, fallbackTitle }: Props = $props();

	function toStandings(raw: unknown): Standings {
		const s = raw as Standings;
		if (!s || !Array.isArray(s.Standings)) throw new Error('Unexpected standings format');
		return s;
	}

	const loader = createLoader(
		() => loadEndpoint(endpoint, toStandings),
		() => peekCache(endpoint, toStandings)
	);

	onMount(() => {
		void loader.refresh();
		return loader.watchVisibility();
	});

	let standings = $derived(loader.data);
	let entries = $derived(
		(standings?.Standings ?? []).map((e) => ({ level: e.Level, teams: sortStandingTeams(e.Teams ?? []) }))
	);

	let selectedLevel = $derived.by(() => {
		const param = page.url.searchParams.get('level');
		return entries.some((e) => e.level === param) ? param : (entries[0]?.level ?? null);
	});

	$effect(() => {
		pageTitle.value = standings?.Title || fallbackTitle;
	});

	function selectLevel(level: string) {
		const url = new URL(page.url);
		url.searchParams.set('level', level);
		void goto(url, { replaceState: true, keepFocus: true, noScroll: true });
	}
</script>

<div class="page">
	<StatusBar result={loader.result} loading={loader.loading} onrefresh={loader.refresh} />

	{#if loader.result?.notAvailable}
		<p class="banner">No {fallbackTitle.toLowerCase()} are published right now.</p>
	{:else if standings}
		{#if standings.Message?.trim()}
			<p class="banner">{standings.Message}</p>
		{/if}

		<div class="segmented level-tabs" role="group" aria-label="Level">
			{#each entries as e (e.level)}
				<button aria-pressed={e.level === selectedLevel} onclick={() => selectLevel(e.level)}>{e.level}</button>
			{/each}
		</div>
		<hr class="rule thick" />

		<div class="levels">
			{#each entries as e (e.level)}
				<section class="level" class:selected={e.level === selectedLevel} aria-label="{e.level} standings">
					<h2>{e.level}</h2>
					<table>
						<thead>
							<tr>
								<th scope="col" class="rank"><span class="visually-hidden">Rank</span>#</th>
								<th scope="col" class="name">Team</th>
								<th scope="col">W</th>
								<th scope="col">L</th>
								<th scope="col">T</th>
								<th scope="col">Pts</th>
							</tr>
						</thead>
						<tbody>
							{#each e.teams as t (t.Team)}
								<tr>
									<td class="rank">{t.Rank}</td>
									<td class="name">
										{t.Team}
										{#if t.Playoff}<span class="playoff" title="Playoff team" aria-label="playoff team">⭐️</span>{/if}
									</td>
									<td>{t.Wins}</td>
									<td>{t.Losses}</td>
									<td>{t.Ties}</td>
									<td class="pts">{teamPoints(t)}</td>
								</tr>
							{/each}
						</tbody>
					</table>
				</section>
			{/each}
		</div>
	{/if}
</div>

<style>
	.level-tabs {
		margin: 8px 0;
		flex-wrap: wrap;
	}

	.level-tabs button {
		flex: 1 1 auto;
		padding: 0.4em 0.6em;
	}

	.level {
		display: none;
	}

	.level.selected {
		display: block;
	}

	.level h2 {
		display: none;
		margin: 0 0 6px;
		font-size: 1.1rem;
		color: var(--accent);
	}

	table {
		width: 100%;
		border-collapse: collapse;
		font-variant-numeric: tabular-nums;
	}

	th {
		font-size: 0.8rem;
		font-weight: 600;
		color: var(--muted);
		text-align: center;
		padding: 4px;
		border-bottom: 2px solid var(--rule);
	}

	td {
		padding: 9px 4px;
		text-align: center;
		border-bottom: 1px solid var(--surface);
	}

	.rank {
		width: 2.2em;
	}

	.name {
		text-align: left;
	}

	td.name {
		color: var(--accent);
		font-weight: 500;
	}

	.pts {
		font-weight: 600;
	}

	.playoff {
		margin-left: 0.3em;
	}

	/* Large screens: all levels in a grid, no tabs. */
	@media (min-width: 1100px) {
		.level-tabs {
			display: none;
		}

		.levels {
			display: grid;
			grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
			gap: 16px 20px;
		}

		.level {
			display: block;
		}

		.level h2 {
			display: block;
		}

		td {
			padding: 6px 3px;
			font-size: 0.9rem;
		}
	}
</style>
