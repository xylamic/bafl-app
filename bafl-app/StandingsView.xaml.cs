namespace bafl_app;

using System.Text.Json;
using bafl_app.library;
using Microsoft.Maui.Controls;

public partial class StandingsView : ContentPage
{
    private bool _isLoading = true;
    private BaflStandings _standings = new BaflStandings();
    private bool _firstLoad = true;
    private bool _isError = false;
    private int _levelIndex = 0;

    protected string _accessUrl = BaflUtilities.STANDINGS_URL;
    protected string _accessCode = App.GetApiKey("standings");

    public StandingsView()
    {
        InitializeComponent();

        // set binding to itself
        BindingContext = this;
        LastUpdated = "Loading...";

        // begin the data load
        Task.Run(async () => { await LoadView(); });
    }

    /// <summary>
    /// Get the title of the page.
    /// </summary>
    public string TitleText
    {
        get
        {
            return _standings.Title;
        }
    }

    public IEnumerable<BaflStandingEntry> Standings
    {
        get
        {
            return _standings.Standings;
        }
    }

    public int LevelsIndex
    {
        get { return _levelIndex; }
        set
        {
            if (value == _levelIndex)
                return;

            _levelIndex = value;

            OnPropertyChanged(nameof(Teams));
        }
    }

    public bool HasMessage
    {
        get { return !String.IsNullOrWhiteSpace(_standings.Message); }
    }

    public string Message
    {
        get { return _standings.Message; }
    }

    public IEnumerable<BaflStandingTeam> Teams
    {
        get
        {
            if (LevelsIndex >= 0 && _standings.Standings.Count > LevelsIndex)
            {
                return _standings.Standings[LevelsIndex].Teams
                    .OrderBy(team => team.Rank)
                    .ThenBy(team => team.Team);
            }
            else
                return new List<BaflStandingTeam>();
        }
    }

    /// <summary>
    /// Get the whether the page is loading.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
    }

    /// <summary>
    /// Get whether the load resulted in an error.
    /// </summary>
    public bool IsError
    {
        get => _isError;
    }

    /// <summary>
    /// The last updated header string.
    /// </summary>
    public string LastUpdated
    {
        get; private set;
    }

    /// <summary>
    /// Load the view data.
    /// </summary>
    /// <returns>The async task.</returns>
    private async Task LoadView()
    {
        try
        {
            // construct the API key
            string code = String.Format("?code={0}", _accessCode);

            // try to read the latest configuration information.
            HttpClient client = new HttpClient();
            string standingContent = await client.GetStringAsync(_accessUrl + code);
            _standings = JsonSerializer.Deserialize<BaflStandings>(standingContent);

            // set the correct week
            if (_standings.Standings.Count > 0 && _firstLoad)
            {
                LevelsIndex = 0;
            }
            else if (!_firstLoad)
            {
                weekPicker.BindingContext = this;
            }

            // set the text for the page header
            LastUpdated = String.Format("V  Updated {0}  V", DateTime.Now.ToLongDateString());

            _isError = false;
            _firstLoad = false;
        }
        catch (Exception ex)
        {
            _isError = true;
            Console.WriteLine(ex.ToString());
            LastUpdated = String.Format("V  Failed load, try again  V", DateTime.Now.ToShortTimeString());
        }

        _isLoading = false;
        OnPropertyChanged(null);
    }

    /// <summary>
    /// The refresh was triggered by the user.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The args.</param>
    private void RefreshView_Refreshing(System.Object sender, System.EventArgs e)
    {
        if (_isLoading)
            return;

        Task.Run(async () =>
        {
            _isLoading = true;
            OnPropertyChanged(nameof(IsLoading));

            await LoadView();
        });
    }
}


