<script lang="ts">
	import { canInstall, requestInstall } from '$lib/install.svelte';

	const DISMISS_KEY = 'bafl-install-banner-dismissed';

	let dismissed = $state(readDismissed());

	function readDismissed(): boolean {
		try {
			return localStorage.getItem(DISMISS_KEY) === '1';
		} catch {
			return false;
		}
	}

	function dismiss() {
		dismissed = true;
		try {
			localStorage.setItem(DISMISS_KEY, '1');
		} catch {
			// Storage blocked; the banner returns next visit.
		}
	}
</script>

{#if canInstall() && !dismissed}
	<aside class="banner-install" aria-label="Install the app">
		<img src="/icons/icon-192.png" alt="" width="40" height="40" />
		<span>Get the BAFL app on your home screen.</span>
		<button class="btn" onclick={requestInstall}>Install</button>
		<button class="icon-btn" aria-label="Dismiss" onclick={dismiss}>✕</button>
	</aside>
{/if}

<style>
	.banner-install {
		display: flex;
		align-items: center;
		gap: 10px;
		max-width: 720px;
		margin: 0 auto 12px;
		padding: 8px 6px 8px 10px;
		border-radius: var(--radius);
		background: var(--banner-bg);
	}

	span {
		flex: 1;
		font-size: 0.95rem;
	}

	.btn {
		min-height: 36px;
		padding: 0.3em 1em;
	}
</style>
