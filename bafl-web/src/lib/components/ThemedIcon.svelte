<script lang="ts">
	interface Props {
		/** Base name in static/img. Themed icons have `_light` and `_dark` variants. */
		name: string;
		themed?: boolean;
		alt?: string;
		size?: number;
	}

	let { name, themed = true, alt = '', size = 24 }: Props = $props();
</script>

{#if themed}
	<picture style:width="{size}px" style:height="{size}px">
		<source srcset="/img/{name}_dark.png" media="(prefers-color-scheme: dark)" />
		<img src="/img/{name}_light.png" {alt} width={size} height={size} />
	</picture>
{:else}
	<img src="/img/{name}.png" {alt} width={size} height={size} style:width="{size}px" style:height="{size}px" />
{/if}

<style>
	picture {
		display: inline-flex;
		flex: none;
	}

	picture img {
		width: 100%;
		height: 100%;
	}

	img {
		object-fit: contain;
		flex: none;
	}
</style>
