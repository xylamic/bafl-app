﻿namespace bafl_app;

using System.Text.Json;
using bafl_app.library;
using Microsoft.Maui.Controls;

public partial class ScheduleView : ContentPage
{
    private bool _isLoading = true;
    private BaflGameCalendar _calendar = new BaflGameCalendar();
    private bool _firstLoad = true;
    private bool _isError = false;
    private int _weekIndex = 0;
    private BaflGameWeek _week = null;

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

    public BaflGameWeek WeeksItem
    {
        get { return _week; }
        set
        {
            if (value == _week)
                return;

            _week = value;
            OnPropertyChanged(nameof(Matchups));
        }
    }

    //public int WeeksIndex
    //{
    //    get { return _weekIndex; }
    //    set
    //    {
    //        if (value == _weekIndex)
    //            return;

    //        _weekIndex = value;

    //        OnPropertyChanged(nameof(Matchups));
    //    }
    //}

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
            if (WeeksItem != null)
                return WeeksItem.Matchups;
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
        // get the current item if applicable
        string weekText = null;
        if (!_firstLoad && WeeksItem != null)
            weekText = WeeksItem.WeekText;

        try
        {
            // construct the API key
            string code = String.Format("?code={0}", _accessCode);

            // try to read the latest configuration information.
            HttpClient client = new HttpClient();
            string scheduleContent = await client.GetStringAsync(_accessUrl + code);
            _calendar = JsonSerializer.Deserialize<BaflGameCalendar>(scheduleContent);

            // set the correct week
            if (_calendar.Weeks.Count > 0 && _firstLoad)
            {
                bool set = false;
                for (int wIndex = 0; wIndex < _calendar.Weeks.Count; wIndex++)
                {
                    if (_calendar.Weeks[wIndex].Date.AddDays(1) > DateTime.Now)
                    {
                        _week = _calendar.Weeks[wIndex];
                        set = true;
                        break;
                    }
                }

                if (!set)
                    _week = _calendar.Weeks[_calendar.Weeks.Count - 1];
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


