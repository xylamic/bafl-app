<script lang="ts">
	import { onMount } from 'svelte';
	import { ageCutoffDate, calculateLevel, LEVEL_REFERENCE, WEIGHT_NOTE } from '$lib/ageLevel';
	import { parseLocalDate } from '$lib/dates';
	import { pageTitle } from '$lib/pageTitle.svelte';

	const cutoff = ageCutoffDate();
	const defaultBirth = new Date(cutoff.getFullYear() - 10, 7, 1);

	function toInputValue(d: Date): string {
		return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
	}

	let birthValue = $state(toInputValue(defaultBirth));
	let isCheer = $state(false);
	let moveUp = $state(false);

	let birth = $derived(parseLocalDate(birthValue));
	let result = $derived(birth ? calculateLevel(birth, cutoff, isCheer, moveUp) : null);

	const cutoffText = cutoff.toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' });

	onMount(() => {
		pageTitle.value = `${cutoff.getFullYear()} Monitor Calculator`;
	});
</script>

<div class="page narrow">
	<p class="lead">Select the team and enter the birthdate to calculate.</p>

	<fieldset class="inputs">
		<legend class="visually-hidden">Calculator inputs</legend>
		<div class="segmented" role="group" aria-label="Program">
			<button aria-pressed={!isCheer} onclick={() => (isCheer = false)}>Football</button>
			<button aria-pressed={isCheer} onclick={() => (isCheer = true)}>Cheer/Drill</button>
		</div>

		<label class="field">
			<span>Birthdate</span>
			<input type="date" bind:value={birthValue} max={toInputValue(new Date())} required />
		</label>

		<label class="check">
			<input type="checkbox" bind:checked={moveUp} />
			<span>Move up a level</span>
		</label>
	</fieldset>

	<section class="result" aria-live="polite">
		<h2>Age on {cutoffText}</h2>
		<p class="value">{result?.age ?? '–'}</p>
		<h2>Level</h2>
		<p class="value">{result?.level ?? '–'}</p>
		<h2>Max Weight for Level (lbs)</h2>
		<p class="value">{result?.weight ?? '–'}</p>
	</section>

	<p class="note">{WEIGHT_NOTE}</p>

	<table class="reference">
		<caption>[Reference] Weights (lbs) by Level</caption>
		<tbody>
			{#each LEVEL_REFERENCE as row (row.label)}
				<tr>
					<th scope="row">{row.label}</th>
					<td>{row.weight}</td>
				</tr>
			{/each}
		</tbody>
	</table>
</div>

<style>
	.lead {
		text-align: center;
		font-size: 1.1rem;
	}

	.inputs {
		display: grid;
		gap: 14px;
		justify-items: center;
		margin: 0 0 14px;
		padding: 14px;
		border: 1px solid var(--rule);
		border-radius: var(--radius);
	}

	.inputs .segmented {
		width: 100%;
		max-width: 360px;
	}

	.field {
		display: grid;
		gap: 4px;
		text-align: center;
	}

	.field input {
		min-height: 44px;
		padding: 0 0.6em;
		border: 1px solid var(--rule);
		border-radius: var(--radius);
		background: var(--bg);
		font-size: 1.1rem;
		letter-spacing: 0.05em;
	}

	.check {
		display: flex;
		align-items: center;
		gap: 10px;
		font-size: 1.05rem;
		min-height: 44px;
	}

	.check input {
		width: 22px;
		height: 22px;
	}

	.result {
		padding: 10px;
		border: 2px solid var(--secondary);
		border-radius: var(--radius);
		text-align: center;
	}

	.result h2 {
		margin: 8px 0 0;
		font-size: 1.05rem;
		font-weight: 500;
	}

	.result .value {
		margin: 2px 0 8px;
		color: var(--secondary);
		font-size: 1.35rem;
		font-weight: 600;
	}

	.note {
		text-align: center;
		font-size: 0.95rem;
	}

	.reference {
		margin: 0 auto;
		border-top: 2px solid var(--tertiary);
		border-collapse: collapse;
	}

	.reference caption {
		padding: 8px 0 4px;
	}

	.reference th,
	.reference td {
		padding: 4px 18px;
		font-weight: normal;
		text-align: center;
	}
</style>
