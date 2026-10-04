<script lang="ts">
	const DISMISS_KEY = 'bafl-install-hint-dismissed';

	function shouldShow(): boolean {
		if (typeof window === 'undefined') return false;
		const ua = navigator.userAgent;
		// iPadOS reports a Mac user agent; touch support tells them apart.
		const isIos = /iPhone|iPad|iPod/.test(ua) || (/Macintosh/.test(ua) && navigator.maxTouchPoints > 1);
		const isStandalone =
			window.matchMedia('(display-mode: standalone)').matches ||
			(navigator as Navigator & { standalone?: boolean }).standalone === true;
		let dismissed = false;
		try {
			dismissed = localStorage.getItem(DISMISS_KEY) === '1';
		} catch {
			// Storage blocked; show the hint.
		}
		return isIos && !isStandalone && !dismissed;
	}

	let visible = $state(shouldShow());

	function dismiss() {
		visible = false;
		try {
			localStorage.setItem(DISMISS_KEY, '1');
		} catch {
			// Nothing to do; the hint returns next visit.
		}
	}
</script>

{#if visible}
	<aside class="hint" aria-label="Install the app">
		<span>Install BAFL: tap <strong>Share</strong>, then <strong>Add to Home Screen</strong>.</span>
		<button class="icon-btn" aria-label="Dismiss" onclick={dismiss}>✕</button>
	</aside>
{/if}

<style>
	.hint {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: 8px;
		margin: 8px 14px 0;
		padding: 6px 6px 6px 12px;
		border-radius: var(--radius);
		background: var(--banner-bg);
		font-size: 0.95rem;
	}
</style>
