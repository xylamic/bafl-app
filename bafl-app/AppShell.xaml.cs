namespace bafl_app;

/// <summary>
/// Application shell.
/// </summary>
public partial class AppShell : Shell
{
    /// <summary>
    /// Construct the App Shell.
    /// </summary>
	public AppShell()
	{
		InitializeComponent();

        try
        {
            double minPixels = Math.Min(DeviceDisplay.MainDisplayInfo.Width, DeviceDisplay.MainDisplayInfo.Height);

            if (DeviceInfo.Idiom == DeviceIdiom.Tablet && minPixels / DeviceDisplay.MainDisplayInfo.Density >= 800)
            {
                this.FlyoutBehavior = FlyoutBehavior.Locked;
            }
            else
            {
                this.FlyoutBehavior = FlyoutBehavior.Flyout;
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Failed to get and set display information.");
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
        await OpenUrl("https://alerts.weather.gov", true);
    }
}

