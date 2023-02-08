using bafl.library;

namespace bafl_app;

public partial class MainPage : ContentPage
{
    private bool _boardShown = true;
    private bool _clubsShown = true;

    public MainPage()
    {
        InitializeComponent();

        VisualStateManager.GoToState(BoardButton, "Off");
        VisualStateManager.GoToState(ContactButton, "Off");
        VisualStateManager.GoToState(ClubListButton, "Off");

        BindingContext = this;

        _ = InitPage();

        Task.Run(() =>
        {
            BoardShown = false;
            ClubsShown = false;
        });
    }

	public async Task InitPage()
	{
		await App.UpdateConfiguration();

		OnPropertyChanged(null);

        ClickTeamsButton();
	}

	public List<BaflClub> BaflClubs
	{
		get { return App.ClubList.Values.ToList(); }
	}

    public List<BaflBoardMember> BaflBoard
    {
        get { return App.BoardMemberList; }
    }

    public bool BoardShown
    {
        get { return _boardShown; }
        private set
        {
            if (_boardShown != value)
            {
                _boardShown = value;
                OnPropertyChanged(nameof(BoardShown));
            }
        }
    }

    public bool ClubsShown
    {
        get { return _clubsShown; }
        private set
        {
            if (_clubsShown != value)
            {
                _clubsShown = value;
                OnPropertyChanged(nameof(ClubsShown));
            }
        }
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
        BoardShown = false;
        VisualStateManager.GoToState(BoardButton, "Off");

        if (ClubsShown)
        {
            ClubsShown = false;
            VisualStateManager.GoToState(ClubListButton, "Off");
            /*Color bgColor = ClubListButton.BackgroundColor;
			ClubListButton.BackgroundColor = ClubListButton.TextColor;
			ClubListButton.TextColor = bgColor;*/
        }
        else
        {
            ClubsShown = true;
            VisualStateManager.GoToState(ClubListButton, "On");
            /*Color bgColor = ClubListButton.BackgroundColor;
			ClubListButton.BackgroundColor = ClubListButton.TextColor;
			ClubListButton.TextColor = bgColor;*/
        }
    }

    void BoardButton_Pressed(System.Object sender, System.EventArgs e)
    {
        ClubsShown = false;
        VisualStateManager.GoToState(ClubListButton, "Off");

        if (BoardShown)
        {
            BoardShown = false;
            VisualStateManager.GoToState(BoardButton, "Off");
            /*Color bgColor = ClubListButton.BackgroundColor;
			ClubListButton.BackgroundColor = ClubListButton.TextColor;
			ClubListButton.TextColor = bgColor;*/
        }
        else
        {
            BoardShown = true;
            VisualStateManager.GoToState(BoardButton, "On");
            /*Color bgColor = ClubListButton.BackgroundColor;
			ClubListButton.BackgroundColor = ClubListButton.TextColor;
			ClubListButton.TextColor = bgColor;*/
        }
    }

    async void ContactButton_Pressed(System.Object sender, System.EventArgs e)
    {
        await OpenUrl("https://www.bayareafootballleague.org/contact");
    }

    async void TapGestureHyperlink_Tapped(System.Object sender, System.EventArgs e)
    {
		Label label = (Label)sender;
        BaflClub club = (BaflClub) label.BindingContext;
		string url = club.Website;

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
