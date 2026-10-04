<script lang="ts">
	import { FEEDBACK_EMAIL } from '$lib/navigation';

	interface Props {
		open: boolean;
	}

	let { open = $bindable() }: Props = $props();

	let dialog: HTMLDialogElement | undefined = $state();
	let copyStatus = $state<'idle' | 'copied' | 'failed'>('idle');

	$effect(() => {
		if (!dialog) return;
		if (open && !dialog.open) {
			copyStatus = 'idle';
			dialog.showModal();
		} else if (!open && dialog.open) {
			dialog.close();
		}
	});

	async function copyAddress() {
		try {
			await navigator.clipboard.writeText(FEEDBACK_EMAIL);
			copyStatus = 'copied';
		} catch {
			copyStatus = 'failed';
		}
	}
</script>

<dialog bind:this={dialog} onclose={() => (open = false)} aria-labelledby="feedback-title">
	<h2 id="feedback-title">Couldn't open your email app?</h2>
	<p>Send your questions or comments to:</p>
	<p class="address">{FEEDBACK_EMAIL}</p>
	{#if copyStatus === 'copied'}
		<p class="note" role="status">Copied. Paste it into your email app.</p>
	{:else if copyStatus === 'failed'}
		<p class="note" role="status">Couldn't copy. Press and hold the address to select it.</p>
	{/if}

	<div class="actions">
		<button class="btn" onclick={copyAddress}>Copy address</button>
		<button class="btn secondary" onclick={() => (open = false)}>Close</button>
	</div>
</dialog>

<style>
	dialog {
		max-width: 380px;
		width: calc(100% - 32px);
		border: none;
		border-radius: var(--radius);
		background: var(--bg);
		color: var(--text);
	}

	dialog::backdrop {
		background: rgba(0, 0, 0, 0.5);
	}

	h2 {
		margin: 0 0 8px;
		font-size: 1.15rem;
		color: var(--accent);
	}

	.address {
		font-size: 1.1rem;
		font-weight: 600;
		text-align: center;
		user-select: all;
		-webkit-user-select: all;
	}

	.note {
		font-size: 0.85rem;
		color: var(--muted);
		text-align: center;
	}

	.actions {
		display: flex;
		gap: 8px;
	}

	.actions .btn {
		flex: 1;
	}
</style>
