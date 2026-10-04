// Chrome/Edge/Samsung fire `beforeinstallprompt`; iOS has no install API, so it gets instructions instead.

interface BeforeInstallPromptEvent extends Event {
	prompt(): Promise<void>;
	userChoice: Promise<{ outcome: 'accepted' | 'dismissed' }>;
}

export type InstallPlatform = 'ios' | 'android' | 'other';

let deferred: BeforeInstallPromptEvent | null = null;
let initialized = false;

export const installState = $state({
	canPrompt: false,
	standalone: false,
	platform: 'other' as InstallPlatform,
	/** Opens the manual-install instructions dialog. */
	showHelp: false
});

function detectPlatform(): InstallPlatform {
	const ua = navigator.userAgent;
	// iPadOS reports a Mac user agent; touch support tells them apart.
	if (/iPhone|iPad|iPod/.test(ua) || (/Macintosh/.test(ua) && navigator.maxTouchPoints > 1)) return 'ios';
	if (/Android/.test(ua)) return 'android';
	return 'other';
}

export function initInstall(): void {
	if (initialized || typeof window === 'undefined') return;
	initialized = true;

	installState.platform = detectPlatform();
	installState.standalone =
		window.matchMedia('(display-mode: standalone)').matches ||
		(navigator as Navigator & { standalone?: boolean }).standalone === true;

	window.addEventListener('beforeinstallprompt', (event) => {
		event.preventDefault();
		deferred = event as BeforeInstallPromptEvent;
		installState.canPrompt = true;
	});

	window.addEventListener('appinstalled', () => {
		deferred = null;
		installState.canPrompt = false;
		installState.standalone = true;
	});
}

/** Whether to offer an Install button at all. */
export function canInstall(): boolean {
	return !installState.standalone && (installState.canPrompt || installState.platform !== 'other');
}

/** Native prompt where the browser supports it; otherwise show manual steps. */
export async function requestInstall(): Promise<void> {
	if (deferred) {
		const event = deferred;
		deferred = null;
		installState.canPrompt = false;
		await event.prompt();
		await event.userChoice;
		return;
	}
	installState.showHelp = true;
}
