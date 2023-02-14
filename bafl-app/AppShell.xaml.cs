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
    protected async Task OpenUrl(string url)
    {
        try
        {
            Uri uri = new Uri(url);
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not open the browser.", "OK");
        }
    }
}

