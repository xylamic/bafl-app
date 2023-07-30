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
        private List<BaflGameMatchupScore> _scores;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflGameMatchup()
        {
            _home = "";
            _away = "";
            _scores = new List<BaflGameMatchupScore>();
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        /// <param name="scores">The scores.</param>
        public BaflGameMatchup(string home, string away, List<BaflGameMatchupScore> scores)
        {
            _home = home;
            _away = away;
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
        /// The score for the match-up.
        /// </summary>
        public List<BaflGameMatchupScore> Scores
        {
            get => _scores;
            set => _scores = value;
        }
    }
}

