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
            _isPlaying = true;
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
        /// The list of selectable numbers.
        /// </summary>
        public int[] Numbers
        {
            get
            {
                // a range from 0 to 99
                int[] numbers = new int[100];
                for (int i = 0; i < 100; i++)
                {
                    numbers[i] = i;
                }
                return numbers;
            }
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
            get
            {
                int targetPlays = PlaysTarget;

                if (_plays >= targetPlays)
                {
                    return targetPlays;
                }
                else
                {
                    return _plays;
                }
            }
            set
            {
                if (SetProperty(ref _plays, value))
                {
                    OnPropertyChanged(nameof(PlaysString));
                    OnPropertyChanged(nameof(PlayStatus));
                }
            }
        }

        /// <summary>
        /// The target number of plays.
        /// </summary>
        public int PlaysTarget
        {
            get
            {
                int targetPlays = _isPeewee ? BaflUtilities.TotalPlaysPw : BaflUtilities.TotalPlays_FrSr;
                if (_halfplays) targetPlays /= 2;
                return targetPlays;
            }
        }

        /// <summary>
        /// The number of plays as a string.
        /// </summary>
        public string PlaysString
        {
            get
            {
                if (Plays >= PlaysTarget)
                {
                    return $"All {PlaysTarget}";
                }
                else
                {
                    return $"{Plays} of {PlaysTarget}";
                }
            }
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
            set
            {
                if (IsPlaying)
                    SetProperty(ref _onfield, value);
            }
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
        /// Add a play.
        /// </summary>
        /// <returns>True if the play was added.</returns>
        public bool AddPlay()
        {
            if (IsPlaying && OnField)
            {
                Plays = _plays + 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Remove a play.
        /// </summary>
        public void RemovePlay()
        {
            if (Plays > 0)
            {
                Plays = _plays - 1;
            }
        }

        public void ResetPlayer()
        {
            Plays = 0;
            HalfPlays = false;
            OnPropertyChanged(nameof(PlaysString));
            OnPropertyChanged(nameof(PlayStatus));
            OnPropertyChanged(nameof(Plays));
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

