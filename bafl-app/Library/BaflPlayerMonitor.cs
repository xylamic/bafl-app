using System;
using System.ComponentModel;
namespace bafl_app.library
{
    /// <summary>
    /// The BAFL play monitor for an individual player.
    /// </summary>
    public class BaflPlayerMonitor : NotifyPropertyChangedBase
    {
        private bool _isPeewee;
        private int _number;
        private string _name;
        private int _plays;
        private bool _halfplays;
        private bool _onfield;
        private bool _isPlaying;

        /// <summary>
        /// The play status for a player.
        /// </summary>
        public enum PlayerPlayStatus
        {
            NoPlays,
            PartialPlays,
            CompletedPlays,
            NotPlaying
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflPlayerMonitor()
        {
            _isPeewee = false;
            _number = 0;
            _name = "";
            _plays = 0;
            _halfplays = false;
            _onfield = false;
            _isPlaying = false;
        }

        /// <summary>
        /// Construct the monitor item.
        /// </summary>
        /// <param name="isPeewee">Whether this is a peewee player.</param>
        /// <param name="number">The player number.</param>
        /// <param name="name">The player name.</param>
        /// <param name="plays">The number of plays.</param>
        /// <param name="halfplays">Whether this play is getting half the plays.</param>
        /// <param name="onfield">Whether this player is on the field.</param>
        /// <param name="isPlaying">Whether this player is playing.</param>
        public BaflPlayerMonitor(bool isPeewee, int number, string name, int plays,
            bool halfplays, bool onfield, bool isPlaying)
        {
            _isPeewee = isPeewee;
            _number = number;
            _name = name;
            _plays = plays;
            _halfplays = halfplays;
            _onfield = onfield;
            _isPlaying = isPlaying;
        }

        /// <summary>
        /// Whether this is a peewee player.
        /// </summary>
        public bool IsPeewee
        {
            get => _isPeewee;
            set => SetProperty(ref _isPeewee, value);
        }

        /// <summary>
        /// The player number.
        /// </summary>
        public int Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        /// <summary>
        /// The player name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// The number of plays.
        /// </summary>
        public int Plays
        {
            get => _plays;
            set => SetProperty(ref _plays, value);
        }

        /// <summary>
        /// Whether this play is getting half the plays.
        /// </summary>
        public bool HalfPlays
        {
            get => _halfplays;
            set
            {
                if (SetProperty(ref _halfplays, value))
                {
                    OnPropertyChanged(nameof(PlayStatus));
                }
            }
        }

        /// <summary>
        /// Whether this player is on the field.
        /// </summary>
        public bool OnField
        {
            get => _onfield;
            set => SetProperty(ref _onfield, value);
        }

        /// <summary>
        /// Whether this player is playing.
        /// </summary>
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (SetProperty(ref _isPlaying, value))
                {
                    OnPropertyChanged(nameof(PlayStatus));
                }
            }
        }

        /// <summary>
        /// The play status for this player.
        /// </summary>
        public PlayerPlayStatus PlayStatus
        {
            get
            {
                if (!_isPlaying)
                {
                    return PlayerPlayStatus.NotPlaying;
                }

                int targetPlays = _isPeewee ? BaflUtilities.TotalPlaysPw : BaflUtilities.TotalPlays_FrSr;
                if (_halfplays)
                {
                    targetPlays /= 2;
                }

                if (_plays <= 0)
                {
                    return PlayerPlayStatus.NoPlays;
                }
                else if (_plays < targetPlays)
                {
                    return PlayerPlayStatus.PartialPlays;
                }
                else
                {
                    return PlayerPlayStatus.CompletedPlays;
                }
            }
        }
    }
}

