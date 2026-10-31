namespace bafl_app;

/// <summary>
/// Application shell.
/// </summary>
public partial class AppShell : Shell
{
    /// <summary>
    /// The smallest window width and height, in device-independent units, at which the side menu stays open.
    /// Covers iPad and the unfolded iPhone Duo, but not phones in either orientation.
    /// </summary>
    private const double LockedFlyoutMinimumSize = 600;

    /// <summary>
    /// The side menu width when it stays open; this is the iPad default.
    /// </summary>
    private const double LockedFlyoutMaximumWidth = 320;

    /// <summary>
    /// The page width to keep beside an open side menu (a standard iPhone width).
    /// </summary>
    private const double LockedFlyoutMinimumContentWidth = 375;

    /// <summary>
    /// The window hosting the shell, tracked for size changes.
    /// </summary>
    private Window _window;

    /// <summary>
    /// Construct the App Shell.
    /// </summary>
	public AppShell()
	{
		InitializeComponent();
	}

    /// <summary>
    /// Track the hosting window so the side menu follows its size.
    /// </summary>
    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (_window != null)
        {
            _window.SizeChanged -= Window_SizeChanged;
        }

        _window = this.Parent as Window;

        if (_window != null)
        {
            _window.SizeChanged += Window_SizeChanged;
            UpdateFlyoutLayout(_window.Width, _window.Height);
        }
    }

    /// <summary>
    /// The window resized (fold/unfold, Split View, rotation).
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    private void Window_SizeChanged(object sender, EventArgs e)
    {
        if (_window != null)
        {
            UpdateFlyoutLayout(_window.Width, _window.Height);
        }
    }

    /// <summary>
    /// Keep the side menu open when the window is large enough (iPad, unfolded iPhone Duo).
    /// </summary>
    /// <param name="width">The window width.</param>
    /// <param name="height">The window height.</param>
    private void UpdateFlyoutLayout(double width, double height)
    {
        if (!(width > 0 && height > 0))
        {
            return;
        }

        bool locked = Math.Min(width, height) >= LockedFlyoutMinimumSize;

        // iOS sizes a phone-idiom flyout (e.g. iPhone Duo) to 80% of the screen, so set the width explicitly.
        double flyoutWidth = locked
            ? Math.Min(LockedFlyoutMaximumWidth, width - LockedFlyoutMinimumContentWidth)
            : -1;

        if (this.FlyoutWidth != flyoutWidth)
        {
            this.FlyoutWidth = flyoutWidth;
        }

        FlyoutBehavior behavior = locked ? FlyoutBehavior.Locked : FlyoutBehavior.Flyout;

        if (this.FlyoutBehavior != behavior)
        {
            bool wasLocked = this.FlyoutBehavior == FlyoutBehavior.Locked;
            this.FlyoutBehavior = behavior;

            // A locked flyout is presented; iOS leaves it open after switching back to Flyout (e.g. on fold).
            if (wasLocked)
            {
                this.FlyoutIsPresented = false;
            }
        }
    }

    /// <summary>
    /// Scoreboard menu item blicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    protected async void FootballScoreboard_Clicked(System.Object sender, System.EventArgs e)
    {
        await OpenUrl("https://www.bayareafootballleague.org/about-us-2");
    }

    /// <summary>
    /// Standings menu item blicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    protected async void FootballStandings_Clicked(System.Object sender, System.EventArgs e)
    {
        await OpenUrl("https://www.bayareafootballleague.org/about-us-3");
    }

    /// <summary>
    /// Open a URL in the app.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    /// <returns>The async task.</returns>
    protected async Task OpenUrl(string url, bool external = false)
    {
        try
        {
            Uri uri = new Uri(url);

            if (external)
            {
#if IOS
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.External);
#else
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
#endif
            }
            else
            {
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not open the browser or app.", "OK");
        }
    }

    /// <summary>
    /// Facebook link clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    protected async void Facebook_Clicked(System.Object sender, System.EventArgs e)
    {
        await OpenUrl("https://www.facebook.com/bafl.youthsports", true);
    }

    /// <summary>
    /// Website link clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    protected async void Website_Clicked(System.Object sender, System.EventArgs e)
    {
        await OpenUrl("https://www.bayareafootballleague.org", true);
    }

    async void Bylaws_Clicked(System.Object sender, System.EventArgs e)
    {
        await OpenUrl("https://www.bayareafootballleague.org/by-laws", true);
    }

    async void NWS_Clicked(object sender, EventArgs e)
    {
        await OpenUrl("https://www.weather.gov/alerts", true);
    }
}

