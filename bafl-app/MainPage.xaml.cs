using bafl.library;

namespace bafl_app;

public partial class MainPage : ContentPage
{
    private bool _boardShown = false;
    private bool _clubsShown = false;
    private bool _scheduleShown = true;

    public MainPage()
    {
        InitializeComponent();

        VisualStateManager.GoToState(BoardButton, "Off");
        VisualStateManager.GoToState(ContactButton, "Off");
        VisualStateManager.GoToState(ClubListButton, "Off");
        VisualStateManager.GoToState(ScheduleButton, "On");

        BindingContext = this;

        _ = InitPage();

        Task.Run(() =>
        {
            BoardShown = false;
            ClubsShown = false;
            ScheduleShown = true;
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
    public List<BaflScheduleItem> ScheduleList
    {
        get { return App.ScheduleList; }
    }

    public bool BoardShown
    {
        get { return _boardShown; }
        private set
        {
            if (_boardShown != value)
            {
                _boardShown = value;
                OnPropertyChanged();
            }
        }
    }
    public bool ScheduleShown
    {
        get { return _scheduleShown; }
        private set
        {
            if (_scheduleShown != value)
            {
                _scheduleShown = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
				"drill, & cheer since 1977. BAFL consists of 17 teams " +
				"across the greater Houston area.";
        }
    }

    void TeamsButton_Pressed(System.Object sender, System.EventArgs e)
    {
		ClickTeamsButton();
    }

    private void ClickButton(int action = 0)
    {
        switch (action)
        {
            case 0:
                BoardShown = false;
                ClubsShown = false;
                ScheduleShown = true;
                VisualStateManager.GoToState(BoardButton, "Off");
                VisualStateManager.GoToState(ClubListButton, "Off");
                VisualStateManager.GoToState(ScheduleButton, "On");
                break;
            case 1:
                BoardShown = false;
                ClubsShown = true;
                ScheduleShown = false;
                VisualStateManager.GoToState(BoardButton, "Off");
                VisualStateManager.GoToState(ClubListButton, "On");
                VisualStateManager.GoToState(ScheduleButton, "Off");
                break;
            case 2:
                BoardShown = true;
                ClubsShown = false;
                ScheduleShown = false;
                VisualStateManager.GoToState(BoardButton, "On");
                VisualStateManager.GoToState(ClubListButton, "Off");
                VisualStateManager.GoToState(ScheduleButton, "Off");
                break;
        }
    }

    void ScheduleButton_Pressed(System.Object sender, System.EventArgs e)
    {
        ClickButton(0);
    }

    void ClickTeamsButton()
	{
        ClickButton(1);
    }

    void BoardButton_Pressed(System.Object sender, System.EventArgs e)
    {
        ClickButton(2);
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
