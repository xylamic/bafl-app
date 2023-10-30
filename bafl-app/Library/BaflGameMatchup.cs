using System;
namespace bafl_app.library
{
    /// <summary>
    /// The BAFL game matchup.
    /// </summary>
    public class BaflGameMatchup
    {
        private string _home;
        private string _away;
        private bool _isNeutral;
        private string _details;
        private List<BaflGameMatchupScore> _scores;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflGameMatchup()
        {
            _home = "";
            _away = "";
            _isNeutral = false;
            _details = "";
            _scores = new List<BaflGameMatchupScore>();
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        /// <param name="home">The home team.</param>
        /// <param name="away">The away team.</param>
        /// <param name="isNeutral">Whether this is a neutral game.</param>
        /// <param name="details">Any matchup details.</param>
        /// <param name="scores">The scores.</param>
        public BaflGameMatchup(string home, string away, bool isNeutral, string details, List<BaflGameMatchupScore> scores)
        {
            _home = home;
            _away = away;
            _isNeutral = isNeutral;
            _details = details;
            _scores = scores;
        }

        /// <summary>
        /// The level.
        /// </summary>
        public string Home
        {
            get => _home;
            set => _home = value;
        }

        /// <summary>
        /// The level.
        /// </summary>
        public string Away
        {
            get => _away;
            set => _away = value;
        }

        /// <summary>
        /// Whether it is a neutral game.
        /// </summary>
        public bool IsNeutral
        {
            get => _isNeutral;
            set => _isNeutral = value;
        }

        /// <summary>
        /// Get the home/away team divider text.
        /// </summary>
        public string GameNeutralityText
        {
            get
            {
                if (IsNeutral)
                    return "vs";
                else
                    return "@";
            }
        }

        /// <summary>
        /// Any matchup details.
        /// </summary>
        public string Details
        {
            get => _details;
            set => _details = value;
        }

        /// <summary>
        /// Get whether the match has details.
        /// </summary>
        public bool HasDetails
        {
            get
            {
                if (String.IsNullOrWhiteSpace(Details))
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// The score for the match-up.
        /// </summary>
        public List<BaflGameMatchupScore> Scores
        {
            get => _scores;
            set => _scores = value;
        }
    }
}

