namespace bafl_app;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	}

    protected async void FootballScoreboard_Clicked(System.Object sender, System.EventArgs e)
    {
        await OpenUrl("https://www.bayareafootballleague.org/about-us-2");
    }

    protected async void FootballStandings_Clicked(System.Object sender, System.EventArgs e)
    {
        await OpenUrl("https://www.bayareafootballleague.org/about-us-3");
    }

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

