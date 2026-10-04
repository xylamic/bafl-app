<script lang="ts">
	import { onMount } from 'svelte';
	import { loadCoreInfo, parseCoreInfo, peekCache } from '$lib/api';
	import { formatLongDate, parseLocalDate, addDays } from '$lib/dates';
	import { mapUrl, safeUrl } from '$lib/links';
	import { createLoader } from '$lib/loader.svelte';
	import { CONTACT_URL } from '$lib/navigation';
	import { pageTitle } from '$lib/pageTitle.svelte';
	import InstallBanner from '$lib/components/InstallBanner.svelte';

	type Tab = 'schedule' | 'teams' | 'board';

	const loader = createLoader(
		() => loadCoreInfo(),
		() => peekCache('coreinfo', parseCoreInfo)
	);
	let tab = $state<Tab>('schedule');

	// League info rarely changes: show the saved copy, update quietly once per visit.
	onMount(() => {
		pageTitle.value = 'Bay Area Football League';
		void loader.refresh();
	});

	let info = $derived(loader.data);
	let clubs = $derived(Object.values(info?.teams ?? {}));
	let yesterday = addDays(new Date(), -1);
	let schedule = $derived(
		(info?.schedule ?? []).map((item) => {
			const date = parseLocalDate(item.Date);
			return { ...item, dateText: formatLongDate(date), past: date !== null && date < yesterday };
		})
	);
</script>

<div class="page">
	<InstallBanner />

	<section class="intro">
		<img src="/img/bafl.png" alt="Bay Area Football League logo" width="125" height="114" />
		<p>
			Teaching and supporting youth through football, drill, &amp; cheer since 1977.
			{#if clubs.length > 0}BAFL consists of {clubs.length} teams across the greater Houston area.{/if}
		</p>
	</section>

	<div class="segmented tabs" role="group" aria-label="Section">
		<button aria-pressed={tab === 'schedule'} onclick={() => (tab = 'schedule')}>Schedule</button>
		<button aria-pressed={tab === 'teams'} onclick={() => (tab = 'teams')}>Teams</button>
		<button aria-pressed={tab === 'board'} onclick={() => (tab = 'board')}>Board</button>
		<a class="btn secondary" href={CONTACT_URL} target="_blank" rel="noopener noreferrer">Contact Us</a>
	</div>

	{#if !info}
		<p class="muted loading">{loader.loading ? 'Loading…' : ''}</p>
	{:else if tab === 'schedule'}
		<ul class="cards">
			{#each schedule as item, i (i)}
				<li class:past={item.past}>
					<div class="title">
						{#if item.Notable}<span aria-label="notable">⭐️</span>{/if}
						{item.Name}
					</div>
					<div>{item.dateText}</div>
					<div class="muted">Location: {item.Location}</div>
				</li>
			{/each}
		</ul>
	{:else if tab === 'teams'}
		<ul class="cards">
			{#each clubs as club (`${club.Region} ${club.Football}`)}
				{@const website = safeUrl(club.Website)}
				{@const map = mapUrl(club.FieldLocation, club.FieldName)}
				<li>
					<div class="title">
						{club.Region}
						{club.Football}
						{#if website}
							<a class="small" href={website} target="_blank" rel="noopener noreferrer">Website</a>
						{/if}
					</div>
					{#if club.President}<div class="muted small">President: {club.President}</div>{/if}
					{#if map}
						<div class="muted small">
							Field: <a href={map} target="_blank" rel="noopener noreferrer">{club.FieldName}</a>
						</div>
					{/if}
				</li>
			{/each}
		</ul>
	{:else}
		<ul class="cards">
			{#each info?.board ?? [] as member, i (i)}
				<li>
					<div class="title">{member.Role}</div>
					<div class="muted">{member.Name}</div>
				</li>
			{/each}
		</ul>
	{/if}
</div>

<style>
	.intro {
		display: flex;
		align-items: center;
		gap: 16px;
		max-width: 720px;
		margin: 4px auto 12px;
	}

	.intro img {
		flex: none;
		width: 110px;
		height: auto;
	}

	.intro p {
		margin: 0;
		text-align: center;
	}

	.tabs {
		max-width: 720px;
		margin: 0 auto 10px;
		flex-wrap: wrap;
	}

	.tabs > * {
		flex: 1 1 40%;
	}

	.cards {
		list-style: none;
		margin: 0;
		padding: 0;
		display: grid;
		grid-template-columns: 1fr;
		gap: 4px;
		text-align: center;
	}

	.cards li {
		padding: 10px 8px;
	}

	.cards li.past {
		opacity: 0.5;
	}

	.loading {
		text-align: center;
	}

	.title {
		font-size: 1.1rem;
		color: var(--accent);
	}

	.small {
		font-size: 0.85rem;
		margin-left: 0.4em;
	}

	.muted.small {
		margin-left: 0;
	}

	@media (min-width: 560px) {
		.tabs > * {
			flex: 1 1 0;
		}
	}

	@media (min-width: 700px) {
		.cards {
			grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
			gap: 12px;
		}

		.cards li {
			border-radius: var(--radius);
			background: var(--surface);
		}
	}
</style>
