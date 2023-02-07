using bafl.library;

namespace bafl_app;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();

        BindingContext = this;

		_ = InitPage();
	}

	public async Task InitPage()
	{
		await App.UpdateConfiguration();

		ClickTeamsButton();

		OnPropertyChanged(null);
	}

	public List<BaflClub> BaflClubs
	{
		get { return App.ClubList.Values.ToList(); }
	}

	public string NavigateText
	{
		get {
			return "Navigate the menu to learn about the league, follow the " +
				"football season, cheer competition, and drill competition.";
		}
	}

	public string DescriptionText
    {
		get
		{
            /*return "Since 1977, the Bay Area Football League has been " +
			"committed to making a difference in the lives of youths all " +
			"throughout the	Houston area. As a leading youth organization, " +
			"we provide children an opportunity to reach their fullest " +
			"potential through football, drill, & cheer. " +
			"We offer the support they need in order to ensure they grow, " +
			"learn, and thrive within their community.";*/

            return "Teaching and support youth through football, " +
				"drill, & cheer since 1977. BAFL consists of 19 teams " +
				"across the greater Houston area.";
        }
    }

    void TeamsButton_Pressed(System.Object sender, System.EventArgs e)
    {
		ClickTeamsButton();
    }

	void ClickTeamsButton()
	{
        if (ClubListView.IsVisible)
        {
            ClubListView.IsVisible = false;
            VisualStateManager.GoToState(ClubListButton, "Off");
            /*Color bgColor = ClubListButton.BackgroundColor;
			ClubListButton.BackgroundColor = ClubListButton.TextColor;
			ClubListButton.TextColor = bgColor;*/
        }
        else
        {
            ClubListView.IsVisible = true;
            VisualStateManager.GoToState(ClubListButton, "On");
            /*Color bgColor = ClubListButton.BackgroundColor;
			ClubListButton.BackgroundColor = ClubListButton.TextColor;
			ClubListButton.TextColor = bgColor;*/
        }
    }

    void BoardButton_Pressed(System.Object sender, System.EventArgs e)
    {
    }

    void ContactButton_Pressed(System.Object sender, System.EventArgs e)
    {
    }

    async void TapGestureHyperlink_Tapped(System.Object sender, System.EventArgs e)
    {
		Label label = (Label)sender;
		string url = label.Text;

		await OpenUrl(url);
    }

    private async Task OpenUrl(string url)
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
