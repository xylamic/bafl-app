namespace bafl_app;

using System;
using System.IO;
using Microsoft.Maui.Storage;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using bafl_app.library;

/// <summary>
/// The Zelle view.
/// </summary>
public partial class PlayMonitorView : ContentPage
{
    private BaflTeamMonitor _teamMonitor = new BaflTeamMonitor();

    /// <summary>
    /// Construct the view.
    /// </summary>
	public PlayMonitorView()
	{
        _teamMonitor.PropertyChanged += (sender, e) =>
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
        };

        LoadData();

        this.BindingContext = this;

        InitializeComponent();
    }

    public string NextPlay
    {
        get => (_teamMonitor.PlayCount + 1).ToString();
    }

    public string PlayerCount
    {
        get => _teamMonitor.PlayCount.ToString();
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
        OnPropertyChanged(nameof(NextPlay));
        OnPropertyChanged(nameof(LastPlay));
        OnPropertyChanged(nameof(UndoAllowed));
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
        try
        {
            string json = Preferences.Get("team-monitor", "");
            if (json.Length > 0)
            {
                _teamMonitor = BaflTeamMonitor.ImportFromJson(json);
                foreach (BaflPlayerMonitor player in _teamMonitor.Players)
                {
                    player.PropertyChanged += (sender, e) =>
                    {
                        PersistData();
                    };
                }
                OnPropertyChanged(null);
            }
        }
        catch (Exception) {}

        if (_teamMonitor == null)
            _teamMonitor = new BaflTeamMonitor();
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
        OnPropertyChanged(nameof(NextPlay));
        OnPropertyChanged(nameof(LastPlay));
        OnPropertyChanged(nameof(UndoAllowed));
    }

    private async void Reset_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Confirm restart", "Restart the play tracking?", "Yes", "No");
        if (!answer)
            return;

        _teamMonitor.ResetPlays();
        OnPropertyChanged(nameof(NextPlay));
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
    }

    private void AddPlayer_Clicked(object sender, EventArgs e)
    {
        BaflPlayerMonitor player = new BaflPlayerMonitor(IsPeewee, 99, "-", 0, false, false, true);
        player.PropertyChanged += (sender, e) =>
        {
            PersistData();
        };
        _teamMonitor.Players.Add(player);
        OnPropertyChanged(nameof(Players));
    }

    private void ActivePlayer_Clicked(object sender, EventArgs e)
    {
        MenuItem button = (MenuItem)sender;
        BaflPlayerMonitor player = (BaflPlayerMonitor)button.BindingContext;
        player.IsPlaying = !player.IsPlaying;
    }

    private void HalfPlayer_Clicked(object sender, EventArgs e)
    {
        MenuItem button = (MenuItem)sender;
        BaflPlayerMonitor player = (BaflPlayerMonitor)button.BindingContext;
        player.IsHalfPlays = !player.IsHalfPlays;
    }

    /// <summary>
    /// Import a team monitor file.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private async void Import_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Error", "Import not yet implemented.", "OK");
        return;

        // Configure the FilePicker to only show .baflmon files
        var options = new PickOptions
        {
            PickerTitle = "Select a BAFL Team Monitor file",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { ".baflmon" } },
                { DevicePlatform.Android, new[] { ".baflmon" } },
                { DevicePlatform.WinUI, new[] { ".baflmon" } },
                { DevicePlatform.macOS, new[] { ".baflmon" } }
            })
        };

        // get the file
        string file = (await FilePicker.Default.PickAsync(options)).FullPath;
        if (file == null)
            return;

        // read the file
        try
        {
            string json = File.ReadAllText(file);
            BaflTeamMonitor monitor = BaflTeamMonitor.ImportFromJson(json);
            if (monitor != null)
            {
                _teamMonitor = monitor;
                OnPropertyChanged(null);
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not read the file.", "OK");
        }
    }

    private async void Export_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Error", "Export not yet implemented.", "OK");
        return;

        // Configure the FilePicker to only show .baflmon files
        var options = new PickOptions
        {
            PickerTitle = "Select a BAFL Team Monitor file",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { ".baflmon" } },
                { DevicePlatform.Android, new[] { ".baflmon" } },
                { DevicePlatform.WinUI, new[] { ".baflmon" } },
                { DevicePlatform.macOS, new[] { ".baflmon" } }
            })
        };

        // get the file
        string file = (await FilePicker.Default.PickAsync(options)).FullPath;
        if (file == null)
            return;

        // write the file
        try
        {
            string json = _teamMonitor.ExportAsJson();
            File.WriteAllText(file, json);
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Could not write the file.", "OK");
        }
    }
}


