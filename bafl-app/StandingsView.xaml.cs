namespace bafl_app;

using System.Text.Json;
using bafl_app.library;
using Microsoft.Maui.Controls;

public class StandingsViewRegular : StandingsView
{
    public StandingsViewRegular() : base(BaflUtilities.STANDINGS_URL)
    { }
}

public class StandingsView9v9 : StandingsView
{
    public StandingsView9v9() : base(BaflUtilities.STANDINGS9V9_URL)
    { }
}

public partial class StandingsView : ContentPage
{
    private bool _isLoading = true;
    private bool _isRefreshing = false;
    private BaflStandings _standings = new BaflStandings();
    private bool _isError = false;
    private BaflStandingEntry _selectedLevel;

    protected string _accessUrl = "";

    public StandingsView(string accessUrl)
    {
        _accessUrl = accessUrl;

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

    public BaflStandingEntry SelectedLevel
    {
        get
        {
            if (_selectedLevel == null && _standings.Standings.Count > 0)
            {
                _selectedLevel = _standings.Standings[0];
            }
            return _selectedLevel;
        }

        set
        {
            _selectedLevel = value;
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
            if (_selectedLevel != null)
            {
                return _selectedLevel.Teams
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
    /// Get whether the page is refreshing.
    /// </summary>
    public bool IsRefreshing
    {
        get => _isRefreshing;
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
    protected async Task LoadView()
    {
        try
        {
            // construct the API key
            string code = String.Format("?code={0}", await App.GetApiKey());

            // try to read the latest configuration information.
            HttpClient client = new HttpClient();
            string standingContent = await client.GetStringAsync(_accessUrl + code);
            _standings = JsonSerializer.Deserialize<BaflStandings>(standingContent);

            // set the text for the page header
            LastUpdated = BaflUtilities.GenerateUpdateMessage(true);

            _isError = false;
        }
        catch (Exception ex)
        {
            _isError = true;
            Console.WriteLine(ex.ToString());
            LastUpdated = BaflUtilities.GenerateErrorMessage(true, ex);
        }
        finally
        {
            _isLoading = false;
            _isRefreshing = false;
            OnPropertyChanged(String.Empty);
        }
    }

    /// <summary>
    /// The refresh was triggered by the user.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The args.</param>
    protected void RefreshView_Refreshing(System.Object sender, System.EventArgs e)
    {
        if (_isLoading || _isRefreshing)
            return;

        Task.Run(async () =>
        {
            _isRefreshing = true;
            OnPropertyChanged(nameof(IsRefreshing));

            await LoadView();
        });
    }
}


