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

        public BaflTeamMonitor(List<BaflPlayerMonitor> players, string homeTeam, string thisTeam, int opposingTeam)
        {
            _players = new ObservableCollection<BaflPlayerMonitor>(players);
            _thisTeam = homeTeam;
            _opposingTeam = thisTeam;
            _playCount = opposingTeam;
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
        /// The players.
        /// </summary>
        public ObservableCollection<BaflPlayerMonitor> Players
        {
            get => _players;
        }
    }
}

