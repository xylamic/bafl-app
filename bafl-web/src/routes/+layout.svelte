<script lang="ts">
	import '../app.css';
	import { page } from '$app/state';
	import { afterNavigate } from '$app/navigation';
	import { NAV_SECTIONS } from '$lib/navigation';
	import { pageTitle } from '$lib/pageTitle.svelte';
	import ThemedIcon from '$lib/components/ThemedIcon.svelte';
	import InstallHint from '$lib/components/InstallHint.svelte';

	let { children } = $props();

	let menuOpen = $state(false);

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
	</nav>

	{#if menuOpen}
		<button class="scrim" aria-label="Close menu" onclick={() => (menuOpen = false)}></button>
	{/if}

	<main>
		<InstallHint />
		{@render children()}
	</main>
</div>

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

	.sidebar a:hover {
		background: var(--surface);
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
