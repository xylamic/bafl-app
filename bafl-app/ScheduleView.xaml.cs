namespace bafl_app;

using System.Text.Json;
using bafl_app.library;
using Microsoft.Maui.Controls;

public partial class ScheduleView : ContentPage
{
    private bool _isLoading = true;
    private BaflGameCalendar _calendar = new BaflGameCalendar();
    private bool _firstLoad = true;
    private bool _isError = false;

    protected string _accessUrl = BaflUtilities.GAMECALENDAR_URL;
    protected string _accessCode = App.GetApiKey("calendar");

    public ScheduleView()
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
            return _calendar.Title;
        }
    }

    public IEnumerable<BaflGameWeek> Weeks
    {
        get
        {
            return _calendar.Weeks;
        }
    }

    public bool HasMessage
    {
        get { return !String.IsNullOrWhiteSpace(_calendar.Message); }
    }

    public string Message
    {
        get { return _calendar.Message; }
    }

    public IEnumerable<BaflGameMatchup> Matchups
    {
        get
        {
            if (weekPicker.SelectedIndex >= 0)
                return _calendar.Weeks[weekPicker.SelectedIndex].Matchups;
            else
                return new List<BaflGameMatchup>();
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
            string scheduleContent = await client.GetStringAsync(_accessUrl + code);
            _calendar = JsonSerializer.Deserialize<BaflGameCalendar>(scheduleContent);

            if (_firstLoad)
            {
                // set the correct week
                if (_calendar.Weeks.Count > 0)
                {
                    bool set = false;
                    for (int wIndex = 0; wIndex < _calendar.Weeks.Count; wIndex++)
                    {
                        if (_calendar.Weeks[wIndex].Date.AddDays(1) > DateTime.Now)
                        {
                            weekPicker.SelectedIndex = wIndex;
                            set = true;
                            break;
                        }
                    }

                    if (!set)
                        weekPicker.SelectedIndex = _calendar.Weeks.Count - 1;
                }
            }
            else
            {
                OnPropertyChanged(nameof(Weeks));
                OnPropertyChanged(nameof(WeeksItem));

                if (_week != null)
                {
                    weekPicker.SelectedItem = (from w in _calendar.Weeks where w.WeekText == _week.WeekText select w).FirstOrDefault();
                }
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
}


