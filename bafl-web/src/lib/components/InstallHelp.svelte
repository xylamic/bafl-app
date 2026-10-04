<script lang="ts">
	import { installState } from '$lib/install.svelte';

	let dialog: HTMLDialogElement | undefined = $state();

	$effect(() => {
		if (!dialog) return;
		if (installState.showHelp && !dialog.open) dialog.showModal();
		else if (!installState.showHelp && dialog.open) dialog.close();
	});
</script>

<dialog bind:this={dialog} onclose={() => (installState.showHelp = false)} aria-labelledby="install-title">
	<h2 id="install-title">Add BAFL to your home screen</h2>

	{#if installState.platform === 'ios'}
		<ol>
			<li>
				Tap the <strong>Share</strong> button
				<svg class="glyph" viewBox="0 0 24 24" aria-label="Share icon">
					<path
						d="M12 3v12M7.5 7.5 12 3l4.5 4.5M5 11v9h14v-9"
						fill="none"
						stroke="currentColor"
						stroke-width="1.8"
						stroke-linecap="round"
						stroke-linejoin="round"
					/>
				</svg>
				in Safari's toolbar.
			</li>
			<li>Scroll down and tap <strong>Add to Home Screen</strong>.</li>
			<li>Tap <strong>Add</strong>.</li>
		</ol>
		<p class="note">In Chrome on iPhone, the Share button is at the top right of the address bar.</p>
	{:else}
		<ol>
			<li>Open the browser menu <strong>⋮</strong>.</li>
			<li>Tap <strong>Install app</strong> or <strong>Add to Home screen</strong>.</li>
			<li>Confirm with <strong>Install</strong>.</li>
		</ol>
	{/if}

	<button class="btn" onclick={() => (installState.showHelp = false)}>Got it</button>
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

	ol {
		padding-left: 1.3em;
		line-height: 1.7;
	}

	.glyph {
		width: 20px;
		height: 20px;
		vertical-align: -4px;
		color: #0a84ff;
	}

	.note {
		font-size: 0.85rem;
		color: var(--muted);
	}

	.btn {
		width: 100%;
	}
</style>
