<script lang="ts">
	import { onMount, tick } from 'svelte';
	import { pageTitle } from '$lib/pageTitle.svelte';
	import ThemedIcon from '$lib/components/ThemedIcon.svelte';
	import {
		MISS_REASONS,
		addPlayer,
		createTeam,
		deletePlayer,
		exportTeamJson,
		formatLastPlay,
		importTeamJson,
		playersOnField,
		playsLabel,
		resetPlays,
		runPlay,
		setAllOnField,
		setOnField,
		setPlaying,
		setTeamPeewee,
		sortPlayers,
		undoAllowed,
		undoPlay,
		type MissReason,
		type PlayerMonitor,
		type TeamMonitor
	} from '$lib/playMonitor';

	// Same key the iOS app uses for its saved preference.
	const STORAGE_KEY = 'team-monitor';
	const MAX_IMPORT_BYTES = 1_000_000;
	const NUMBERS = Array.from({ length: 100 }, (_, i) => i);
	const REASONS = MISS_REASONS.filter((r) => r !== 'NotSet');

	function loadTeam(): TeamMonitor {
		try {
			const json = localStorage.getItem(STORAGE_KEY);
			if (json) return importTeamJson(json);
		} catch {
			// Unreadable saved data; start fresh like iOS does.
		}
		return createTeam();
	}

	let team = $state<TeamMonitor>(loadTeam());
	let locked = $state(false);
	let countdown = $state(0);
	let openRow = $state<number | null>(null);
	let fileInput: HTMLInputElement | undefined = $state();

	let reasonPrompt = $state<{ title: string; resolve: (r: MissReason | null) => void } | null>(null);
	let reasonDialog: HTMLDialogElement | undefined = $state();

	let onField = $derived(playersOnField(team));
	let onFieldTone = $derived(onField < 11 ? 'under' : onField === 11 ? 'exact' : 'over');

	onMount(() => {
		pageTitle.value = `${new Date().getFullYear()} BAFL Play Monitor`;
	});

	$effect(() => {
		const json = exportTeamJson(team);
		try {
			localStorage.setItem(STORAGE_KEY, json);
		} catch {
			// Storage full or blocked; the session still works.
		}
	});

	async function onRunPlay() {
		if (onField !== 11 && !confirm('Number of players on field is not 11, proceed?')) return;
		runPlay(team);
		navigator.vibrate?.(200);
		locked = true;
		for (let i = 3; i >= 0; i--) {
			countdown = i;
			await new Promise((r) => setTimeout(r, 1000));
		}
		locked = false;
	}

	function onReset() {
		if (confirm('Restart the play tracking?')) resetPlays(team);
	}

	function onSort() {
		if (confirm('Would you like to sort the players by numbers?')) sortPlayers(team);
	}

	async function onAdd() {
		const p = addPlayer(team);
		await tick();
		document.getElementById(`name-${p.id}`)?.focus();
	}

	function chooseReason(title: string): Promise<MissReason | null> {
		return new Promise((resolve) => {
			reasonPrompt = { title, resolve };
			void tick().then(() => reasonDialog?.showModal());
		});
	}

	function finishReason(reason: MissReason | null) {
		reasonPrompt?.resolve(reason);
		reasonPrompt = null;
		reasonDialog?.close();
	}

	async function onInOut(p: PlayerMonitor) {
		openRow = null;
		if (p.isPlaying) {
			const reason = await chooseReason('Select the reason to remove the player.');
			if (!reason) return;
			p.notPlayReason = reason;
			setPlaying(p, false);
		} else if (confirm('Reinstate player into the game?')) {
			p.notPlayReason = 'NotSet';
			setPlaying(p, true);
		}
	}

	async function onHalf(p: PlayerMonitor) {
		openRow = null;
		if (!p.isPlaying) {
			alert('Player must be active to set half-plays.');
			return;
		}
		if (!p.halfPlays) {
			const reason = await chooseReason('Select the reason to set player to half-plays.');
			if (!reason) return;
			p.notPlayReason = reason;
			p.halfPlays = true;
		} else if (confirm('Reinstate player as a full game?')) {
			p.halfPlays = false;
			p.notPlayReason = 'NotSet';
		}
	}

	function onDelete(p: PlayerMonitor) {
		openRow = null;
		if (confirm(`Delete player #${p.number} ${p.name}?`)) deletePlayer(team, p.id);
	}

	async function onImportFile(event: Event) {
		const input = event.currentTarget as HTMLInputElement;
		const file = input.files?.[0];
		input.value = '';
		if (!file) return;
		try {
			if (file.size > MAX_IMPORT_BYTES) throw new Error('File too large');
			team = importTeamJson(await file.text());
		} catch {
			alert('Could not read the file.');
		}
	}

	async function onExport() {
		const name = `${team.thisTeam.replace(/[\\/:*?"<>|]/g, '_').trim() || 'team'}.json`;
		const file = new File([exportTeamJson(team)], name, { type: 'application/json' });
		try {
			if (navigator.canShare?.({ files: [file] })) {
				await navigator.share({ files: [file], title: 'Share team monitor file' });
				return;
			}
		} catch (err) {
			if (err instanceof DOMException && err.name === 'AbortError') return;
		}
		const url = URL.createObjectURL(file);
		const a = document.createElement('a');
		a.href = url;
		a.download = name;
		a.click();
		setTimeout(() => URL.revokeObjectURL(url), 1000);
	}
</script>

<div class="page monitor">
	<div class="toolbar">
		<span class="stat">Plays: <strong>{team.playCount}</strong></span>
		<button class="icon-btn" aria-label="Undo last play" disabled={!undoAllowed(team) || locked} onclick={() => undoPlay(team)}>
			<ThemedIcon name="undo" />
		</button>
		<button class="icon-btn" aria-label="Restart play tracking" disabled={locked} onclick={onReset}>
			<ThemedIcon name="restart" />
		</button>
		<span class="spacer"></span>
		<span class="stat">Players: <strong>{team.players.length}</strong></span>
		<button class="icon-btn" aria-label="Sort players by number" onclick={onSort}>
			<ThemedIcon name="sortbynumber" />
		</button>
		<button class="icon-btn" aria-label="Add player" onclick={onAdd}>
			<ThemedIcon name="plus" />
		</button>
		<button class="icon-btn" aria-label="Import team file" onclick={() => fileInput?.click()}>
			<ThemedIcon name="import" />
		</button>
		<button class="icon-btn" aria-label="Export team file" onclick={onExport}>
			<ThemedIcon name="export" />
		</button>
		<input bind:this={fileInput} type="file" accept=".json,application/json" hidden onchange={onImportFile} />
	</div>
	<hr class="rule thick" />

	<div class="header row" aria-hidden="true">
		<span>#</span><span>On Field</span><span>Name</span><span class="left">Left</span><span></span>
	</div>

	<ul class="players">
		{#each team.players as p (p.id)}
			<li class:out={!p.isPlaying}>
				<div class="row">
					<select bind:value={p.number} aria-label="Jersey number">
						{#each NUMBERS as n (n)}<option value={n}>{n}</option>{/each}
					</select>
					<label class="switch">
						<input
							type="checkbox"
							checked={p.onField}
							disabled={!p.isPlaying}
							onchange={(e) => setOnField(p, e.currentTarget.checked)}
							aria-label="On field"
						/>
						<span aria-hidden="true"></span>
					</label>
					<input id="name-{p.id}" class="name" bind:value={p.name} aria-label="Player name" />
					<span class="left">{playsLabel(p)}{p.halfPlays ? ' ½' : ''}</span>
					<button
						class="icon-btn more"
						aria-label="Player actions"
						aria-expanded={openRow === p.id}
						onclick={() => (openRow = openRow === p.id ? null : p.id)}>⋯</button
					>
				</div>
				{#if openRow === p.id}
					<div class="actions">
						<button class="btn danger" onclick={() => onDelete(p)}>Delete</button>
						<button class="btn inout" onclick={() => onInOut(p)}>In/out</button>
						<button class="btn half" onclick={() => onHalf(p)}>Half</button>
					</div>
				{/if}
			</li>
		{:else}
			<li class="empty muted">No players yet. Use + to add players or import a team file.</li>
		{/each}
	</ul>

	<div class="footer">
		<div class="footer-row">
			<button class="icon-btn" aria-label="All players off the field" onclick={() => setAllOnField(team, false)}>
				<ThemedIcon name="leftswitch" size={30} />
			</button>
			<button class="icon-btn" aria-label="All players on the field" onclick={() => setAllOnField(team, true)}>
				<ThemedIcon name="rightswitch" size={30} />
			</button>
			<span class="onfield">On Field: <strong class={onFieldTone}>{onField}</strong></span>
			<span class="spacer"></span>
			<button class="btn run" disabled={locked} onclick={onRunPlay}>Run Play</button>
		</div>
		<div class="footer-row small">
			<span>Last: {formatLastPlay(team.lastPlay)}</span>
			<span class="spacer"></span>
			<label class="peewee">
				<input type="checkbox" checked={team.isPeewee} onchange={(e) => setTeamPeewee(team, e.currentTarget.checked)} />
				Peewee
			</label>
		</div>
		<div class="teams">
			<input bind:value={team.thisTeam} aria-label="This team" />
			<span>vs.</span>
			<input bind:value={team.opposingTeam} aria-label="Opposing team" />
		</div>
	</div>
</div>

{#if locked}
	<div class="overlay" role="alert" aria-live="assertive">{countdown}</div>
{/if}

<dialog bind:this={reasonDialog} onclose={() => reasonPrompt && finishReason(null)}>
	{#if reasonPrompt}
		<p>{reasonPrompt.title}</p>
		<div class="reasons">
			{#each REASONS as r (r)}
				<button class="btn secondary" onclick={() => finishReason(r)}>{r}</button>
			{/each}
		</div>
		<button class="btn" onclick={() => finishReason(null)}>Cancel</button>
	{/if}
</dialog>

<style>
	.monitor {
		max-width: 900px;
		padding-bottom: 0;
	}

	.toolbar {
		display: flex;
		align-items: center;
		gap: 0;
	}

	.toolbar .icon-btn {
		width: 36px;
		height: 40px;
		padding: 6px 4px;
	}

	.stat {
		margin: 0 4px 0 2px;
		white-space: nowrap;
	}

	.spacer {
		flex: 1;
	}

	.row {
		display: grid;
		grid-template-columns: 64px 56px 1fr 72px 40px;
		align-items: center;
		gap: 8px;
	}

	.header {
		font-size: 0.85rem;
		color: var(--muted);
		text-align: center;
	}

	.header .left {
		font-size: inherit;
	}

	.players {
		list-style: none;
		margin: 0;
		padding: 0;
	}

	.players li {
		padding: 6px 0;
		border-bottom: 1px solid var(--rule);
	}

	.players li.out .name,
	.players li.out select {
		opacity: 0.55;
	}

	.empty {
		text-align: center;
		padding: 24px 0 !important;
	}

	select,
	.name,
	.teams input {
		min-height: 42px;
		border: 1px solid var(--surface);
		border-radius: 8px;
		background: var(--bg);
	}

	select {
		font-size: 1.2rem;
		text-align: center;
	}

	.name {
		min-width: 0;
		padding: 0 0.5em;
		font-size: 1.15rem;
	}

	.left {
		text-align: center;
		font-size: 1.05rem;
		white-space: nowrap;
	}

	.more {
		font-size: 1.4rem;
	}

	.switch {
		position: relative;
		display: inline-block;
		width: 52px;
		height: 32px;
		justify-self: center;
	}

	.switch input {
		position: absolute;
		inset: 0;
		opacity: 0;
		margin: 0;
		cursor: pointer;
	}

	.switch span {
		position: absolute;
		inset: 0;
		border-radius: 16px;
		background: var(--gray-300);
		transition: background 0.15s;
		pointer-events: none;
	}

	.switch span::after {
		content: '';
		position: absolute;
		top: 3px;
		left: 3px;
		width: 26px;
		height: 26px;
		border-radius: 50%;
		background: #fff;
		transition: transform 0.15s;
	}

	.switch input:checked + span {
		background: var(--primary);
	}

	.switch input:checked + span::after {
		transform: translateX(20px);
	}

	.switch input:disabled + span {
		opacity: 0.4;
	}

	.switch input:focus-visible + span {
		outline: 2px solid var(--secondary);
		outline-offset: 2px;
	}

	.actions {
		display: flex;
		justify-content: flex-end;
		gap: 8px;
		padding-top: 6px;
	}

	.actions .btn {
		border: none;
		color: #fff;
	}

	.btn.danger {
		background: #d00000;
	}

	.btn.inout {
		background: #e08000;
	}

	.btn.half {
		background: #1f5fd1;
	}

	.footer {
		position: sticky;
		bottom: 0;
		padding: 8px 4px calc(10px + env(safe-area-inset-bottom));
		background: var(--bg);
		border-top: 4px solid var(--rule);
	}

	.footer-row {
		display: flex;
		align-items: center;
		gap: 6px;
	}

	.footer-row.small {
		margin-top: 6px;
		font-size: 0.9rem;
	}

	.onfield {
		font-size: 1.2rem;
		margin-left: 6px;
	}

	.under {
		color: var(--warn);
	}

	.exact {
		color: var(--ok);
	}

	.over {
		color: var(--danger);
	}

	.run {
		min-height: 48px;
		padding: 0 1.4em;
		font-size: 1.1rem;
		font-weight: 600;
	}

	.peewee {
		display: flex;
		align-items: center;
		gap: 6px;
	}

	.peewee input {
		width: 20px;
		height: 20px;
	}

	.teams {
		display: grid;
		grid-template-columns: 1fr auto 1fr;
		align-items: center;
		gap: 8px;
		margin-top: 8px;
	}

	.teams input {
		min-width: 0;
		padding: 0 0.5em;
		text-align: center;
	}

	.overlay {
		position: fixed;
		inset: 0;
		z-index: 100;
		display: flex;
		align-items: center;
		justify-content: center;
		background: rgba(0, 0, 0, 0.6);
		color: #fff;
		font-size: 4rem;
		font-weight: 600;
	}

	dialog {
		max-width: 360px;
		width: calc(100% - 32px);
		border: none;
		border-radius: var(--radius);
		background: var(--bg);
		color: var(--text);
		text-align: center;
	}

	dialog::backdrop {
		background: rgba(0, 0, 0, 0.5);
	}

	.reasons {
		display: grid;
		gap: 8px;
		margin-bottom: 12px;
	}

	@media (max-width: 420px) {
		.row {
			grid-template-columns: 56px 52px 1fr 60px 36px;
			gap: 6px;
		}
	}
</style>
