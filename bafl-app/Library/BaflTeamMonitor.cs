using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace bafl_app.library
{
    /// <summary>
    /// The BAFL play monitor for an individual player.
    /// </summary>
    public class BaflTeamMonitor : NotifyPropertyChangedBase
    {
        private ObservableCollection<BaflPlayerMonitor> _players;
        private string _thisTeam;
        private string _opposingTeam;
        private int _playCount;
        private List<BaflPlayerMonitor> _undoPlayers = new List<BaflPlayerMonitor>();

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflTeamMonitor()
        {
            _players = new ObservableCollection<BaflPlayerMonitor>();
            _thisTeam = "";
            _opposingTeam = "";
            _playCount = 0;
            LastPlay = null;
        }

        public BaflTeamMonitor(IEnumerable<BaflPlayerMonitor> players, string thisTeam, string opposingTeam, int playCount)
        {
            _players = new ObservableCollection<BaflPlayerMonitor>(players);
            _thisTeam = thisTeam;
            _opposingTeam = opposingTeam;
            _playCount = playCount;
        }

        /// <summary>
        /// This team.
        /// </summary>
        public string ThisTeam
        {
            get => _thisTeam;
            set => SetProperty(ref _thisTeam, value);
        }

        /// <summary>
        /// The opposing team.
        /// </summary>
        public string OpposingTeam
        {
            get => _opposingTeam;
            set => SetProperty(ref _opposingTeam, value);
        }

        /// <summary>
        /// The play count.
        /// </summary>
        public int PlayCount
        {
            get => _playCount;
            set => SetProperty(ref _playCount, value);
        }

        /// <summary>
        /// The player count.
        /// </summary>
        public int PlayerCount
        {
            get => _players.Count;
        }

        public DateTime? LastPlay
        {
            get;
            set;
        }

        /// <summary>
        /// Get whether undo is allowed.
        /// </summary>
        public bool UndoAllowed
        {
            get
            {
                return _undoPlayers.Count > 0;
            }
        }

        /// <summary>
        /// Get whether the team has completed its plays.
        /// </summary>
        public bool TeamComplete
        {
            get
            {
                foreach (BaflPlayerMonitor player in _players)
                {
                    if (player.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NoPlays ||
                        player.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Get the number of players on the field.
        /// </summary>
        public int PlayersOnField
        {
            get => _players.Count(p => p.OnField && p.IsPlaying);
        }

        /// <summary>
        /// Run the play.
        /// </summary>
        public void RunPlay()
        {
            ClearUndo();

            bool playRun = false;
            foreach (BaflPlayerMonitor player in _players)
            {
                if (player.AddPlay())
                {
                    _undoPlayers.Add(player);
                    playRun = true;
                }
            }
            
            if (playRun)
            {
                PlayCount += 1;
                OnPropertyChanged(nameof(UndoAllowed));
                LastPlay = DateTime.Now;
            }
        }

        /// <summary>
        /// Undo the play.
        /// </summary>
        public void UndoPlay()
        {
            if (_undoPlayers.Count > 0)
            {
                foreach (BaflPlayerMonitor player in _undoPlayers)
                {
                    player.RemovePlay();
                }
                PlayCount -= 1;
                ClearUndo();
            }
        }

        /// <summary>
        /// Sort the players.
        /// </summary>
        public void SortPlayers()
        {
            var sorted = Players.OrderBy(x => x.Number).ToList();
            Players.Clear();

            foreach (var item in sorted)
            {
                Players.Add(item);
            }

            OnPropertyChanged(nameof(Players));
        }

        public void SetAllOnField(bool onField)
        {
            foreach (BaflPlayerMonitor player in _players)
            {
                player.OnField = onField;
            }
        }

        /// <summary>
        /// Reset the plays for the whole team.
        /// </summary>
        public void ResetPlays()
        {
            foreach (BaflPlayerMonitor player in _players)
            {
                player.ResetPlayer();
            }
            ClearUndo();
            LastPlay = null;
            PlayCount = 0;
        }

        /// <summary>
        /// The players.
        /// </summary>
        public ObservableCollection<BaflPlayerMonitor> Players
        {
            get => _players;
        }

        /// <summary>
        /// Clear the undo buffer.
        /// </summary>
        private void ClearUndo()
        {
            _undoPlayers.Clear();
            OnPropertyChanged(nameof(UndoAllowed));
        }
    }
}

