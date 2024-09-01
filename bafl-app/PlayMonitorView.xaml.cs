namespace bafl_app;

using System;
using System.IO;
using Microsoft.Maui.Storage;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using bafl_app.library;
using System.ComponentModel;
using Microsoft.Maui.Devices;

/// <summary>
/// The Zelle view.
/// </summary>
public partial class PlayMonitorView : ContentPage
{
    private BaflTeamMonitor _teamMonitor = new BaflTeamMonitor();
    private bool _screenLocked = false;
    private string _countdownValue = "";

    /// <summary>
    /// Construct the view.
    /// </summary>
	public PlayMonitorView()
	{
        LoadData();

        this.BindingContext = this;

        InitializeComponent();
    }

    public string PlayCount
    {
        get => _teamMonitor.PlayCount.ToString();
    }

    public string PlayerCount
    {
        get => _teamMonitor.PlayerCount.ToString();
    }

    /// <summary>
    /// Get and set whether the screen is locked.
    /// </summary>
    public bool Locked
    {
        get => _screenLocked;
        set
        {
            _screenLocked = value;
            OnPropertyChanged(nameof(Locked));
            OnPropertyChanged(nameof(NotLocked));
        }
    }

    public bool NotLocked
    {
        get => !_screenLocked;
    }

    /// <summary>
    /// Get the countdown value on the screen.
    /// </summary>
    public string CountdownValue
    {
        get => _countdownValue;
        set
        {
            _countdownValue = value;
            OnPropertyChanged(nameof(CountdownValue));
        }
    }

    public bool IsPeewee
    {
        get => _teamMonitor.IsPeewee;
        set
        {
            _teamMonitor.IsPeewee = value;
            foreach (BaflPlayerMonitor player in _teamMonitor.Players)
            {
                player.IsPeewee = value;
            }
            OnPropertyChanged(nameof(IsPeewee));
        }
    }

    public string LastPlay
    {
        get
        {
            if (_teamMonitor.LastPlay == null)
            {
                return "No plays";
            }
            else
            {
                return _teamMonitor.LastPlay.Value.ToString("yyyy-M-d h:mm tt");
            }
        }
    }

    public string OnFieldCount
    {
        get
        {
            return $"{_teamMonitor.PlayersOnField}";
        }
    }

    /// <summary>
    /// Get the color for the number of players on the field.
    /// </summary>
    public Color OnFieldColor
    {
        get
        {
            if (_teamMonitor.PlayersOnField < 11)
            {
                return Color.FromRgb(255, 69, 0);
            }
            else if (_teamMonitor.PlayersOnField == 11)
            {
                return Color.FromRgb(0, 200, 0);
            }
            else
            {
                return Color.FromRgb(200, 0, 0);
            }
        }
    }

    /// <summary>
    /// Get the team name.
    /// </summary>
    public string ThisTeam
    {
        get => _teamMonitor.ThisTeam;
        set
        {
            _teamMonitor.ThisTeam = value;
            OnPropertyChanged(nameof(ThisTeam));
        }
    }

    /// <summary>
    /// Get the opposing team name.
    /// </summary>
    public string OpposingTeam
    {
        get => _teamMonitor.OpposingTeam;
        set
        {
            _teamMonitor.OpposingTeam = value;
            OnPropertyChanged(nameof(OpposingTeam));
        }
    }

    public ObservableCollection<BaflPlayerMonitor> Players
    {
        get => _teamMonitor.Players;
    }

    public bool UndoAllowed
    {
        get => _teamMonitor.UndoAllowed;
    }

    private async void RunPlayButton_Clicked(object sender, EventArgs e)
    {
        if (_teamMonitor.PlayersOnField != 11)
        {
            bool answer = await DisplayAlert("Confirm run", "Number of players on field is not 11, proceed?", "Yes", "No");
            if (!answer)
                return;
        }

        _teamMonitor.RunPlay();
        Vibration.Default.Vibrate();
        OnPropertyChanged(nameof(PlayCount));
        OnPropertyChanged(nameof(LastPlay));
        OnPropertyChanged(nameof(UndoAllowed));

        // Show the countdown label
        Locked = true;

        // Countdown from 3 to 0
        for (int i = 3; i >= 0; i--)
        {
            CountdownValue = i.ToString();

            // Wait for 1 second
            await Task.Delay(1000);
        }

        // Hide the countdown label or reset it
        Locked = false;
    }

    /// <summary>
    /// Persist the data.
    /// </summary>
    private void PersistData()
    {
        Preferences.Set("team-monitor", _teamMonitor.ExportAsJson());
    }

    /// <summary>
    /// Load the data.
    /// </summary>
    private void LoadData()
    {
        _teamMonitor = null;
        try
        {
            string json = Preferences.Get("team-monitor", "");
            if (json.Length > 0)
            {
                _teamMonitor = BaflTeamMonitor.ImportFromJson(json);
                _teamMonitor.PropertyChanged += TeamPropertyChange;
                foreach (BaflPlayerMonitor player in _teamMonitor.Players)
                {
                    player.PropertyChanged += PlayerPropertyChange;
                }
                OnPropertyChanged(String.Empty);
            }
        }
        catch (Exception) {}

        if (_teamMonitor == null)
        {
            _teamMonitor = new BaflTeamMonitor();
            _teamMonitor.PropertyChanged += TeamPropertyChange;
        }
    }

    private void OnFieldSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        /*Switch sw = (Switch)sender;
        BaflPlayerMonitor player = (BaflPlayerMonitor)sw.BindingContext;
        player.OnField = sw.IsToggled;*/

        OnPropertyChanged(nameof(OnFieldCount));
        OnPropertyChanged(nameof(OnFieldColor));
    }

    private void AllOffField_Clicked(object sender, EventArgs e)
    {
        _teamMonitor.SetAllOnField(false);
        OnPropertyChanged(nameof(OnFieldCount));
    }

    private void AllOnField_Clicked(object sender, EventArgs e)
    {
        _teamMonitor.SetAllOnField(true);
        OnPropertyChanged(nameof(OnFieldCount));
    }

    private void Undo_Clicked(object sender, EventArgs e)
    {
        _teamMonitor.UndoPlay();
        OnPropertyChanged(nameof(PlayCount));
        OnPropertyChanged(nameof(LastPlay));
        OnPropertyChanged(nameof(UndoAllowed));
    }

    private async void Reset_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Confirm restart", "Restart the play tracking?", "Yes", "No");
        if (!answer)
            return;

        _teamMonitor.ResetPlays();
        OnPropertyChanged(nameof(PlayCount));
        OnPropertyChanged(nameof(LastPlay));
    }

    private async void SortByNumber_Clicked(object sender, EventArgs e)
    {
        // confirm the sort with the user
        bool answer = await DisplayAlert("Confirm sort", "Would you like to sort the players by numbers?", "Yes", "No");
        if (answer)
        {
            _teamMonitor.SortPlayers();
            OnPropertyChanged(nameof(Players));
            OnPropertyChanged(nameof(PlayerCount));
        }
    }

    private void DeletePlayer_Clicked(object sender, EventArgs e)
    {
        MenuItem button = (MenuItem)sender;
        BaflPlayerMonitor player = (BaflPlayerMonitor)button.BindingContext;
        _teamMonitor.Players.Remove(player);
        OnPropertyChanged(nameof(Players));
        OnPropertyChanged(nameof(PlayerCount));
    }

    private void AddPlayer_Clicked(object sender, EventArgs e)
    {
        BaflPlayerMonitor player = new BaflPlayerMonitor(IsPeewee, 99, "-", 0, false, false, true);
        player.PropertyChanged += PlayerPropertyChange;
        _teamMonitor.Players.Add(player);
        OnPropertyChanged(nameof(Players));
        OnPropertyChanged(nameof(PlayerCount));
    }

    private async void ActivePlayer_Clicked(object sender, EventArgs e)
    {
        MenuItem button = (MenuItem)sender;
        BaflPlayerMonitor player = (BaflPlayerMonitor)button.BindingContext;

        if (player.IsPlaying)
        {
            string[] vals = Enum.GetNames(typeof(BaflPlayerMonitor.PlayerMissReasons))[1..];
            string action = await DisplayActionSheet("Select the reason to remove the player.", "Cancel", null, vals);
            if (action == "Cancel")
                return;

            // find the index of action in vals
            int index = Array.IndexOf(vals, action) + 1;
            if (index <= 0)
                index = 0;
            
            player.NotPlayReason = (BaflPlayerMonitor.PlayerMissReasons)index;

            player.IsPlaying = false;
        }
        else
        {
            bool answer = await DisplayAlert("Confirm", "Reinstate player into the game?", "Yes", "No");
            if (answer)
            {
                player.IsPlaying = false;
                player.NotPlayReason = BaflPlayerMonitor.PlayerMissReasons.NotSet;
            }

            player.IsPlaying = true;
        }

        OnPropertyChanged(nameof(OnFieldCount));
    }

    private async void HalfPlayer_Clicked(object sender, EventArgs e)
    {
        MenuItem button = (MenuItem)sender;
        BaflPlayerMonitor player = (BaflPlayerMonitor)button.BindingContext;

        if (!player.IsPlaying)
        {
            await DisplayAlert("Error", "Player must be active to set half-plays.", "OK");
            return;
        }

        if (!player.IsHalfPlays)
        {
            string[] vals = Enum.GetNames(typeof(BaflPlayerMonitor.PlayerMissReasons))[1..];
            string action = await DisplayActionSheet("Select the reason to set player to half-plays.", "Cancel", null, vals);
            if (action == "Cancel")
                return;

            int index = Array.IndexOf(vals, action) + 1;
            if (index <= 0)
                index = 0;
            
            player.NotPlayReason = (BaflPlayerMonitor.PlayerMissReasons)index;

            player.IsHalfPlays = true;
        }
        else
        {
            bool answer = await DisplayAlert("Confirm", "Reinstate player as a full game?", "Yes", "No");
            if (answer)
            {
                player.IsHalfPlays = false;
                player.NotPlayReason = BaflPlayerMonitor.PlayerMissReasons.NotSet;
            }
            
            player.IsHalfPlays = false;
        }
    }

    /// <summary>
    /// The team property change events.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void TeamPropertyChange(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BaflTeamMonitor.PlayersOnField))
            OnPropertyChanged(nameof(OnFieldCount));
        else if (e.PropertyName == nameof(BaflTeamMonitor.UndoAllowed))
            OnPropertyChanged(nameof(UndoAllowed));
        else if (e.PropertyName == nameof(BaflTeamMonitor.LastPlay))
            OnPropertyChanged(nameof(LastPlay));
        else if (e.PropertyName == nameof(BaflTeamMonitor.IsPeewee))
            OnPropertyChanged(nameof(IsPeewee));

        PersistData();
    }

    /// <summary>
    /// Property change events for a player.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void PlayerPropertyChange(object sender, PropertyChangedEventArgs e)
    {
        PersistData();
    }

    /// <summary>
    /// Import a team monitor file.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private async void Import_Clicked(object sender, EventArgs e)
    {
        string file = null;
        try
        {
            // Configure the FilePicker to only show .baflmon files
            var options = new PickOptions
            {
                PickerTitle = "Select a BAFL Team Monitor file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } },
                    { DevicePlatform.macOS, new[] { "public.json" } }
                })
            };

            // get the file
            file = (await FilePicker.Default.PickAsync(options)).FullPath;
            if (string.IsNullOrWhiteSpace(file))
                return;
        }
        catch (Exception)
        {
            return;
        }

        // read the file
        try
        {
            string json = File.ReadAllText(file);
            BaflTeamMonitor monitor = BaflTeamMonitor.ImportFromJson(json);
            if (monitor != null)
            {
                _teamMonitor = monitor;

                _teamMonitor.PropertyChanged += TeamPropertyChange;
                foreach (BaflPlayerMonitor player in _teamMonitor.Players)
                {
                    player.PropertyChanged += PlayerPropertyChange;
                }

                OnPropertyChanged(String.Empty);
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not read the file.", "OK");
        }
    }

    /// <summary>
    /// Export the team monitor file.
    /// </summary>
    private async void Export_Clicked(object sender, EventArgs e)
    {
        try
        {
            string json = _teamMonitor.ExportAsJson();
            string file = await BaflUtilities.CreateFileAsync(ThisTeam + ".json", json);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Share team monitor file",
                File = new ShareFile(file, "application/json")
            });
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not write & share the file.", "OK");
        }
    }

    private async void ThisTeam_Tapped(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("This team", "What's this team's name?", "OK", "Cancel", ThisTeam);
        if (!String.IsNullOrWhiteSpace(result))
        {
            ThisTeam = result;
        }
    }

    private async void OpposingTeam_Tapped(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Opposing team", "What's the opposing team's name?", "OK", "Cancel", OpposingTeam);
        if (!String.IsNullOrWhiteSpace(result))
        {
            OpposingTeam = result;
        }
    }
}


