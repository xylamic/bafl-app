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

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflTeamMonitor()
        {
            _players = new ObservableCollection<BaflPlayerMonitor>();
            _thisTeam = "";
            _opposingTeam = "";
            _playCount = 0;
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

        public void RunPlay()
        {
        }

        /// <summary>
        /// The players.
        /// </summary>
        public ObservableCollection<BaflPlayerMonitor> Players
        {
            get => _players;
        }
    }
}

