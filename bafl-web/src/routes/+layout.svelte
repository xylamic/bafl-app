<script lang="ts">
	import '../app.css';
	import { page } from '$app/state';
	import { afterNavigate } from '$app/navigation';
	import { NAV_SECTIONS, feedbackMailto } from '$lib/navigation';
	import { pageTitle } from '$lib/pageTitle.svelte';
	import ThemedIcon from '$lib/components/ThemedIcon.svelte';
	import InstallHelp from '$lib/components/InstallHelp.svelte';
	import FeedbackHelp from '$lib/components/FeedbackHelp.svelte';
	import { canInstall, initInstall, requestInstall } from '$lib/install.svelte';

	let { children } = $props();

	// Register before the browser fires `beforeinstallprompt` shortly after load.
	initInstall();

	let menuOpen = $state(false);
	let feedbackHref = $derived(feedbackMailto(page.url.pathname, navigator.userAgent));
	let showFeedbackHelp = $state(false);

	// Browsers don't report whether a mailto: link opened anything. If the page never loses
	// focus to a mail app, assume nothing opened and offer the address instead.
	function onFeedbackClick() {
		menuOpen = false;
		let left = false;
		const markLeft = () => {
			left = true;
		};
		window.addEventListener('blur', markLeft);
		window.addEventListener('pagehide', markLeft);
		document.addEventListener('visibilitychange', markLeft);
		setTimeout(() => {
			window.removeEventListener('blur', markLeft);
			window.removeEventListener('pagehide', markLeft);
			document.removeEventListener('visibilitychange', markLeft);
			if (!left && document.visibilityState === 'visible' && document.hasFocus()) showFeedbackHelp = true;
		}, 2000);
	}

	afterNavigate(() => {
		menuOpen = false;
	});

	function onKeydown(event: KeyboardEvent) {
		if (event.key === 'Escape') menuOpen = false;
	}
</script>

<svelte:window onkeydown={onKeydown} />

<svelte:head>
	<title>{pageTitle.value === 'BAFL' ? 'BAFL' : `${pageTitle.value} · BAFL`}</title>
</svelte:head>

<div class="shell">
	<header class="topbar">
		<button
			class="icon-btn menu-btn"
			aria-label="Open menu"
			aria-expanded={menuOpen}
			aria-controls="sidebar"
			onclick={() => (menuOpen = !menuOpen)}
		>
			<svg viewBox="0 0 24 24" width="24" height="24" aria-hidden="true">
				<path d="M3 6h18M3 12h18M3 18h18" stroke="currentColor" stroke-width="2" stroke-linecap="round" />
			</svg>
		</button>
		<h1>{pageTitle.value}</h1>
	</header>

	<nav id="sidebar" class="sidebar" class:open={menuOpen} aria-label="Main">
		{#each NAV_SECTIONS as section, i (i)}
			{#if i > 0}<hr />{/if}
			<ul>
				{#each section as link (link.href)}
					<li>
						<a
							href={link.href}
							class:active={!link.external && page.url.pathname === link.href}
							aria-current={!link.external && page.url.pathname === link.href ? 'page' : undefined}
							target={link.external ? '_blank' : undefined}
							rel={link.external ? 'noopener noreferrer' : undefined}
						>
							<ThemedIcon name={link.icon} themed={link.themed} size={26} />
							<span>{link.label}</span>
							{#if link.external}<span class="ext" aria-label="opens in a new tab">↗</span>{/if}
						</a>
					</li>
				{/each}
			</ul>
		{/each}
		<hr />
		<a href={feedbackHref} onclick={onFeedbackClick}>
			<svg class="menu-svg" viewBox="0 0 24 24" width="26" height="26" aria-hidden="true">
				<path
					d="M4 5.5A2.5 2.5 0 0 1 6.5 3h11A2.5 2.5 0 0 1 20 5.5v8a2.5 2.5 0 0 1-2.5 2.5H11l-4.5 4v-4h0A2.5 2.5 0 0 1 4 13.5z"
					fill="none"
					stroke="currentColor"
					stroke-width="1.6"
					stroke-linejoin="round"
				/>
				<path d="M8 8.5h8M8 11.5h5" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" />
			</svg>
			<span>Send Feedback</span>
		</a>
		{#if canInstall()}
			<hr />
			<button
				class="install-item"
				onclick={() => {
					menuOpen = false;
					void requestInstall();
				}}
			>
				<svg viewBox="0 0 24 24" width="26" height="26" aria-hidden="true">
					<rect x="6" y="2.5" width="12" height="19" rx="2.5" fill="none" stroke="currentColor" stroke-width="1.6" />
					<path d="M12 7v8M8.5 11.5 12 15l3.5-3.5" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" stroke-linejoin="round" />
				</svg>
				<span>Install App</span>
			</button>
		{/if}
	</nav>

	{#if menuOpen}
		<button class="scrim" aria-label="Close menu" onclick={() => (menuOpen = false)}></button>
	{/if}

	<main>
		{@render children()}
	</main>
</div>

<InstallHelp />
<FeedbackHelp bind:open={showFeedbackHelp} />

<style>
	.topbar {
		position: sticky;
		top: 0;
		z-index: 20;
		display: flex;
		align-items: center;
		gap: 8px;
		height: calc(var(--topbar-height) + env(safe-area-inset-top));
		padding: env(safe-area-inset-top) 8px 0 calc(8px + env(safe-area-inset-left));
		background: var(--primary);
		color: #fff;
	}

	.topbar h1 {
		margin: 0;
		font-size: 1.15rem;
		font-weight: 600;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}

	.menu-btn {
		color: #fff;
	}

	.menu-btn:hover {
		background: rgba(255, 255, 255, 0.12) !important;
	}

	.sidebar {
		position: fixed;
		z-index: 40;
		top: 0;
		bottom: 0;
		left: 0;
		width: min(var(--sidebar-width), 85vw);
		overflow-y: auto;
		padding: calc(12px + env(safe-area-inset-top)) 8px 24px calc(8px + env(safe-area-inset-left));
		background: var(--bg);
		border-right: 1px solid var(--surface);
		transform: translateX(-100%);
		transition: transform 0.2s ease;
	}

	.sidebar.open {
		transform: translateX(0);
		box-shadow: 4px 0 24px rgba(0, 0, 0, 0.3);
	}

	.sidebar ul {
		list-style: none;
		margin: 0;
		padding: 0;
	}

	.sidebar hr {
		border: none;
		border-top: 1px solid var(--surface);
		margin: 8px 4px;
	}

	.sidebar a {
		display: flex;
		align-items: center;
		gap: 14px;
		min-height: 44px;
		padding: 6px 10px;
		border-radius: 8px;
		color: var(--text);
		text-decoration: none;
	}

	.sidebar a:hover,
	.install-item:hover {
		background: var(--surface);
	}

	.install-item {
		display: flex;
		align-items: center;
		gap: 14px;
		width: 100%;
		min-height: 44px;
		padding: 6px 10px;
		border: none;
		border-radius: 8px;
		background: transparent;
		color: var(--accent);
		font-weight: 600;
		text-align: left;
		cursor: pointer;
	}

	.sidebar a.active {
		background: var(--banner-bg);
		font-weight: 600;
	}

	.ext {
		margin-left: auto;
		color: var(--muted);
		font-size: 0.85em;
	}

	.menu-svg {
		flex: none;
	}

	.scrim {
		position: fixed;
		inset: 0;
		z-index: 30;
		border: none;
		background: rgba(0, 0, 0, 0.45);
	}

	main {
		padding-bottom: env(safe-area-inset-bottom);
	}

	/* Wide screens: the menu stays open, like the iPad's locked flyout. */
	@media (min-width: 900px) {
		.sidebar {
			transform: none;
			box-shadow: none;
			top: 0;
		}

		.sidebar.open {
			box-shadow: none;
		}

		.topbar,
		main {
			margin-left: var(--sidebar-width);
		}

		.menu-btn,
		.scrim {
			display: none;
		}

		.topbar {
			padding-left: 20px;
		}
	}
</style>
