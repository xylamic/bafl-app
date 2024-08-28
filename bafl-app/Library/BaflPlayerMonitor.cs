using System;
using System.ComponentModel;
using System.Dynamic;
namespace bafl_app.library
{
    /// <summary>
    /// The BAFL play monitor for an individual player.
    /// </summary>
    public class BaflPlayerMonitor : NotifyPropertyChangedBase
    {
        #region Private Variables
        private bool _isPeewee;
        private int _number;
        private string _name;
        private int _plays;
        private bool _halfplays;
        private bool _onfield;
        private bool _isPlaying;
        private PlayerMissReasons _notPlayReasons;
        #endregion

        #region Serialization
        /// <summary>
        /// The static data of a player for serialization.
        /// </summary>
        public class SBaflPlayerMonitor
        {
            public bool IsPeewee {get; set;}
            public int Number {get; set;}
            public string Name {get; set;}
            public int Plays {get; set;}
            public bool HalfPlays {get; set;}
            public bool OnField {get; set;}
            public bool IsPlaying {get; set;}
            public int NotPlayReason { get; set; }
        }

        /// <summary>
        /// Construct the monitor item from a JSON string.
        /// </summary>
        /// <returns>The JSON string.</returns>
        public string ExportAsJson()
        {
            SBaflPlayerMonitor sBaflPlayerMonitor = new SBaflPlayerMonitor()
            {
                IsPeewee = IsPeewee,
                Number = Number,
                Name = Name,
                Plays = Plays,
                HalfPlays = IsHalfPlays,
                OnField = OnField,
                IsPlaying = IsPlaying,
                NotPlayReason = (int)NotPlayReason
            };
            string json = System.Text.Json.JsonSerializer.Serialize(sBaflPlayerMonitor);
            return json;
        }

        /// <summary>
        /// Import the monitor item from a JSON string.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>The monitor item.</returns>
        public static BaflPlayerMonitor ImportFromJson(string json)
        {
            SBaflPlayerMonitor spm = System.Text.Json.JsonSerializer.Deserialize<SBaflPlayerMonitor>(json);
            return new BaflPlayerMonitor(spm.IsPeewee, spm.Number, spm.Name,
                spm.Plays, spm.HalfPlays, spm.OnField, spm.IsPlaying, (PlayerMissReasons)spm.NotPlayReason);
        }
        #endregion

        #region Enums
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
        /// Enum defining why a player is not getting plays.
        /// </summary>
        public enum PlayerMissReasons
        {
            NotSet = 0,
            Injured = 1,
            Absent = 2,
            Discipline = 3,
            Sick = 4,
            ParentRequest = 5,
            Ejected = 6
        }
        #endregion

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
            _notPlayReasons = PlayerMissReasons.NotSet;
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
        /// <param name="notPlayReason">The reason why a player may not be playing.</param>
        public BaflPlayerMonitor(bool isPeewee, int number, string name, int plays,
            bool halfplays, bool onfield, bool isPlaying, PlayerMissReasons notPlayReason = PlayerMissReasons.NotSet)
        {
            _isPeewee = isPeewee;
            _number = number;
            _name = name;
            _plays = plays;
            _halfplays = halfplays;
            _onfield = onfield;
            _isPlaying = isPlaying;
            _notPlayReasons = notPlayReason;
        }

        /// <summary>
        /// Whether this is a peewee player.
        /// </summary>
        public bool IsPeewee
        {
            get => _isPeewee;
            set
            {
                SetProperty(ref _isPeewee, value);
                OnPropertyChanged(nameof(PlaysTarget));
                OnPropertyChanged(nameof(PlaysString));
                OnPropertyChanged(nameof(PlayStatus));
            }
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
        /// The reason a player may not be playing.
        /// </summary>
        public PlayerMissReasons NotPlayReason
        {
            get; set;
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
                if (!IsPlaying)
                {
                    return "N/A";
                }
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
        public bool IsHalfPlays
        {
            get => _halfplays;
            set
            {
                if (SetProperty(ref _halfplays, value))
                {
                    OnPropertyChanged(nameof(PlayStatus));
                    OnPropertyChanged(nameof(PlaysString));
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
                    OnPropertyChanged(nameof(PlaysString));

                    if (!_isPlaying)
                    {
                        OnField = false;
                        OnPropertyChanged(nameof(OnField));
                    }
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
            IsHalfPlays = false;
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

