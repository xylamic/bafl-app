using System;
namespace bafl_app.library
{
    /// <summary>
    /// The BAFL game score.
    /// </summary>
    public class BaflGameMatchupScore
    {
        private string _level;
        private string _score;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflGameMatchupScore()
        {
            _level = "";
            _score = "";
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="score">The score.</param>
        public BaflGameMatchupScore(string level, string score)
        {
            _level = level;
            _score = score;
        }

        /// <summary>
        /// The level.
        /// </summary>
        public string Level
        {
            get => _level;
            set => _level = value;
        }

        /// <summary>
        /// The score for the match-up.
        /// </summary>
        public string Score
        {
            get => _score;
            set => _score = value;
        }
    }
}

