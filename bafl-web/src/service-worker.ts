/// <reference types="@sveltejs/kit" />
/// <reference no-default-lib="true"/>
/// <reference lib="esnext" />
/// <reference lib="webworker" />
import { build, files, version } from '$service-worker';

const sw = self as unknown as ServiceWorkerGlobalScope;
const CACHE = `bafl-${version}`;
// SWA does not serve its own config file; precaching it would fail the install.
const ASSETS = [...build, ...files].filter((f) => f !== '/staticwebapp.config.json');

sw.addEventListener('install', (event) => {
	event.waitUntil(
		caches
			.open(CACHE)
			.then((cache) => cache.addAll([...ASSETS, '/']))
			.then(() => sw.skipWaiting())
	);
});

sw.addEventListener('activate', (event) => {
	event.waitUntil(
		caches
			.keys()
			.then((keys) => Promise.all(keys.filter((k) => k !== CACHE).map((k) => caches.delete(k))))
			.then(() => sw.clients.claim())
	);
});

sw.addEventListener('fetch', (event) => {
	const request = event.request;
	if (request.method !== 'GET') return;

	const url = new URL(request.url);
	// API data is cached by the app in localStorage; only same-origin app files are handled here.
	if (url.origin !== sw.location.origin || url.pathname.startsWith('/api/')) return;

	if (ASSETS.includes(url.pathname)) {
		event.respondWith(caches.match(url.pathname).then((hit) => hit ?? fetch(request)));
		return;
	}

	if (request.mode === 'navigate') {
		// Network first so deploys show up; fall back to the cached app shell offline.
		event.respondWith(
			fetch(request).catch(async () => (await caches.match('/')) ?? Response.error())
		);
	}
});
