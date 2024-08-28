namespace bafl_app;

using System;
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
        };

        _teamMonitor.Players.Add(new BaflPlayerMonitor(false, 1, "John Doe", 0, false, false, true));
        _teamMonitor.Players.Add(new BaflPlayerMonitor(false, 2, "Jane Doe", 0, false, false, true));
        _teamMonitor.Players.Add(new BaflPlayerMonitor(false, 3, "Jim Doe", 0, false, false, true));
        _teamMonitor.Players.Add(new BaflPlayerMonitor(false, 99, "Jill Doe", 0, false, false, true));
        _teamMonitor.Players.Add(new BaflPlayerMonitor(false, 1, "Jack Doe", 0, false, false, true));

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
                return _teamMonitor.LastPlay.Value.ToLongTimeString();
            }
        }
    }

    public string OnFieldCount
    {
        get
        {
            return $"On field: {_teamMonitor.PlayersOnField}";
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

    private void RunPlayButton_Clicked(object sender, EventArgs e)
    {
        _teamMonitor.RunPlay();
        OnPropertyChanged(nameof(NextPlay));
        OnPropertyChanged(nameof(LastPlay));
    }

    private void OnFieldSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        /*Switch sw = (Switch)sender;
        BaflPlayerMonitor player = (BaflPlayerMonitor)sw.BindingContext;
        player.OnField = sw.IsToggled;*/

        OnPropertyChanged(nameof(OnFieldCount));
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
    }

    private void Reset_Clicked(object sender, EventArgs e)
    {
        _teamMonitor.ResetPlays();
        OnPropertyChanged(nameof(NextPlay));
        OnPropertyChanged(nameof(LastPlay));
    }

    private async void SortByNumber_Clicked(object sender, EventArgs e)
    {
        // confirm the sort with the user
        bool answer = await DisplayAlert("Confirm", "Would you like to sort the players by numbers?", "Yes", "No");
        if (answer)
        {
            _teamMonitor.SortPlayers();
            OnPropertyChanged(nameof(Players));
            OnPropertyChanged(nameof(PlayerCount));
        }
    }
}


