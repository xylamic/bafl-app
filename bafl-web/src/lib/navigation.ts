// Menu order and links mirror bafl-app/AppShell.xaml and AppShell.xaml.cs.

export interface NavLink {
	label: string;
	href: string;
	/** Base name of the light/dark icon pair in static/img, or a single icon file. */
	icon: string;
	themed: boolean;
	external?: boolean;
}

export const NAV_SECTIONS: NavLink[][] = [
	[{ label: 'Bay Area Football League', href: '/', icon: 'bafl', themed: false }],
	[
		{ label: 'Season Schedule', href: '/schedule', icon: 'schedule', themed: true },
		{ label: 'Season Standings', href: '/standings', icon: 'standings', themed: true },
		{ label: 'Cheer Competition', href: '/cheer', icon: 'cheer', themed: true },
		{ label: 'Drill Competition', href: '/drill', icon: 'drill', themed: true }
	],
	[
		{ label: 'BAFL Website', href: 'https://www.bayareafootballleague.org', icon: 'website', themed: true, external: true },
		{ label: 'BAFL Facebook', href: 'https://www.facebook.com/bafl.youthsports', icon: 'facebook', themed: false, external: true },
		{ label: 'BAFL By-laws', href: 'https://www.bayareafootballleague.org/by-laws', icon: 'bylaws', themed: true, external: true },
		{ label: 'BAFL Monitor', href: '/age-weight', icon: 'calc', themed: true },
		{ label: 'Play Monitor', href: '/play-monitor', icon: 'playmonitor', themed: true },
		{ label: 'NWS Alerts', href: 'https://www.weather.gov/alerts', icon: 'nws', themed: false, external: true }
	]
];

export const CONTACT_URL = 'https://www.bayareafootballleague.org/contact';

export const FEEDBACK_EMAIL = 'bafl@xylasoft.com';

/** Pre-fills the subject plus the page and browser so app issues are easier to reproduce. */
export function feedbackMailto(path: string, userAgent: string): string {
	const subject = 'BAFL App Feedback';
	const body = ['', '', '---', `Page: ${path}`, `Browser: ${userAgent}`].join('\r\n');
	return `mailto:${FEEDBACK_EMAIL}?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`;
}
