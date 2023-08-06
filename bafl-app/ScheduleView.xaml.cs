﻿namespace bafl_app;

using System.Text.Json;
using bafl_app.library;
using Microsoft.Maui.Controls;

public partial class ScheduleView : ContentPage
{
    private bool _isLoading = true;
    private BaflGameCalendar _calendar = new BaflGameCalendar();
    private bool _isError = false;
    private BaflGameWeek _selectedWeek = null;
    private bool _isRefreshing = false;

    protected string _accessUrl = BaflUtilities.GAMECALENDAR_URL;

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

    public BaflGameWeek SelectedWeek
    {
        get
        {
            if (_selectedWeek == null && _calendar.Weeks.Count > 0)
            {
                _selectedWeek = _calendar.Weeks[FindClosestWeek()];
            }
            return _selectedWeek;
        }

        set
        {
            _selectedWeek = value;
            OnPropertyChanged(nameof(Matchups));
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
    /// Find the closest week date.
    /// </summary>
    /// <returns>The week index.</returns>
    private int FindClosestWeek()
    {
        for (int index = 0; index < _calendar.Weeks.Count; index++)
        {
            if (_calendar.Weeks[index].Date.AddDays(1) > DateTime.Now)
                return index;
        }

        if (_calendar.Weeks.Count > 0)
        {
            return _calendar.Weeks.Count - 1;
        }
        else
        {
            return -1;
        }
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
            string code = String.Format("?code={0}", await App.GetApiKey());

            // try to read the latest configuration information.
            HttpClient client = new HttpClient();
            string scheduleContent = await client.GetStringAsync(_accessUrl + code);
            _calendar = JsonSerializer.Deserialize<BaflGameCalendar>(scheduleContent);            

            // set the text for the page header
            LastUpdated = String.Format("V  Updated {0}  V", DateTime.Now.ToLongDateString());

            _isError = false;
        }
        catch (Exception ex)
        {
            _isError = true;
            Console.WriteLine(ex.ToString());
            LastUpdated = String.Format("V  Failed load, try again  V", DateTime.Now.ToShortTimeString());
        }

        _isLoading = false;
        _isRefreshing = false;
        OnPropertyChanged(null);
    }

    /// <summary>
    /// The refresh was triggered by the user.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The args.</param>
    private void RefreshView_Refreshing(System.Object sender, System.EventArgs e)
    {
        if (_isLoading || _isRefreshing)
            return;

        Task.Run(async () =>
        {
            _isRefreshing = true;
            OnPropertyChanged(nameof(IsLoading));
            OnPropertyChanged(nameof(IsRefreshing));

            await LoadView();
        });
    }
}


