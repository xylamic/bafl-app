import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vitest/config';

export default defineConfig({
	plugins: [sveltekit()],
	server: {
		// The Function App has no CORS rule for localhost yet, so dev calls go through this proxy.
		proxy: {
			'/api': {
				target: 'https://baflapp.azurewebsites.net',
				changeOrigin: true
			}
		}
	},
	test: {
		include: ['src/**/*.test.ts'],
		environment: 'node'
	}
});
