using bafl_app.library;

namespace bafl_app;

/// <summary>
/// The Main Page of the app.
/// </summary>
public partial class MainPage : ContentPage
{
    private bool _boardShown = true;
    private bool _clubsShown = true;
    private bool _scheduleShown = true;

    /// <summary>
    /// Construct the view.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();

        // Turn all buttons to the off state, except the schedule
        VisualStateManager.GoToState(BoardButton, "Off");
        VisualStateManager.GoToState(ContactButton, "Off");
        VisualStateManager.GoToState(ClubListButton, "Off");
        VisualStateManager.GoToState(ScheduleButton, "On");

        BindingContext = this;

        // initialize the page view
        _ = InitPage();

        // set the visibility to match the buttons
        Task.Run(() =>
        {
            BoardShown = false;
            ClubsShown = false;
            ScheduleShown = true;
        });
    }

    /// <summary>
    /// Initialize the page.
    /// </summary>
    /// <returns>The async task.</returns>
	public async Task InitPage()
	{
        // Update the configuration for the app.
		await App.LoadConfiguration();

        // trigger the bindings to update
		OnPropertyChanged(null);
    }

    /// <summary>
    /// The list of BAFL clubs.
    /// </summary>
	public List<BaflClub> BaflClubs
	{
		get { return App.ClubList.Values.ToList(); }
	}

    /// <summary>
    /// The list of BAFL board members.
    /// </summary>
    public List<BaflBoardMember> BaflBoard
    {
        get { return App.BoardMemberList; }
    }

    /// <summary>
    /// The list of schedule items.
    /// </summary>
    public List<BaflScheduleItem> ScheduleList
    {
        get { return App.ScheduleList; }
    }

    /// <summary>
    /// Whether the board members is being shown.
    /// </summary>
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

    /// <summary>
    /// Whether the schedule is being shown.
    /// </summary>
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

    /// <summary>
    /// Whether the clubs are being shown.
    /// </summary>
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

    /// <summary>
    /// Navigate text.
    /// </summary>
	public string NavigateText
	{
		get {
			return "Navigate the menu to learn about the league, follow the " +
				"football season, cheer competition, and drill competition.";
		}
	}

    /// <summary>
    /// BAfL description text.
    /// </summary>
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

            int numTeams = BaflClubs.Count;

            return String.Format("Teaching and support youth through football, " +
				"drill, & cheer since 1977. BAFL consists of {0} teams " +
				"across the greater Houston area.", numTeams);
        }
    }

    /// <summary>
    /// The team list button was pressed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    void TeamsButton_Pressed(System.Object sender, System.EventArgs e)
    {
        ClickButton(1);
    }

    /// <summary>
    /// A button was clicked.
    /// </summary>
    /// <param name="action">0=schedule, 1=teams, 2=board</param>
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

    /// <summary>
    /// The schedule list button was pressed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    void ScheduleButton_Pressed(System.Object sender, System.EventArgs e)
    {
        ClickButton(0);
    }

    /// <summary>
    /// The board list button was pressed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    void BoardButton_Pressed(System.Object sender, System.EventArgs e)
    {
        ClickButton(2);
    }

    /// <summary>
    /// The contact button was pressed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    async void ContactButton_Pressed(System.Object sender, System.EventArgs e)
    {
        await OpenUrl("https://www.bayareafootballleague.org/contact");
    }

    /// <summary>
    /// The links were tapped for a given team.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The args.</param>
    async void TapGestureHyperlink_Tapped(System.Object sender, System.EventArgs e)
    {
		Label label = (Label)sender;
        BaflClub club = (BaflClub) label.BindingContext;
		string url = club.Website;

		await OpenUrl(url);
    }

    /// <summary>
    /// Open a URL in the app.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    /// <returns>The async task.</returns>
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
