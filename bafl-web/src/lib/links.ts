/** Only http(s) URLs from API data are rendered as links. */
export function safeUrl(url: string | null | undefined): string | null {
	if (!url) return null;
	try {
		const parsed = new URL(url.trim());
		return parsed.protocol === 'https:' || parsed.protocol === 'http:' ? parsed.href : null;
	} catch {
		return null;
	}
}

function isApplePlatform(): boolean {
	return typeof navigator !== 'undefined' && /iPhone|iPad|iPod|Macintosh/.test(navigator.userAgent);
}

/** Map link for a "lat, long" string; Apple Maps on Apple devices, Google Maps elsewhere. */
export function mapUrl(location: string | null | undefined, name: string): string | null {
	if (!location) return null;
	const parts = location.split(',').map((s) => Number.parseFloat(s.trim()));
	const [lat, lng] = parts;
	if (parts.length !== 2 || lat === undefined || lng === undefined || !Number.isFinite(lat) || !Number.isFinite(lng)) {
		return null;
	}
	const q = encodeURIComponent(name || `${lat},${lng}`);
	return isApplePlatform()
		? `https://maps.apple.com/?ll=${lat},${lng}&q=${q}`
		: `https://www.google.com/maps/search/?api=1&query=${lat},${lng}`;
}
