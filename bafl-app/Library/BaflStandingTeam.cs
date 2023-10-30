using System;
namespace bafl_app.library
{
    /// <summary>
    /// The BAFL game calendar.
    /// </summary>
    public class BaflStandingTeam
    {
        private string _team;
        private int _wins;
        private int _losses;
        private int _ties;
        private bool _playoff;
        private int _rank;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflStandingTeam()
        {
            _team = "";
            _wins = 0;
            _losses = 0;
            _ties = 0;
            _playoff = false;
            _rank = 0;
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflStandingTeam(string team, int wins, int losses,
            int ties, bool playoff, int rank)
        {
            _team = team;
            _wins = wins;
            _losses = losses;
            _ties = ties;
            _playoff = playoff;
            _rank = rank;
        }

        /// <summary>
        /// The team name.
        /// </summary>
        public string Team
        {
            get => _team;
            set => _team = value;
        }

        /// <summary>
        /// The team wins.
        /// </summary>
        public int Wins
        {
            get => _wins;
            set => _wins = value;
        }

        /// <summary>
        /// Wins readable text.
        /// </summary>
        public string WinsText
        {
            get { return String.Format("{0}W", Wins); }
        }

        /// <summary>
        /// The team losses.
        /// </summary>
        public int Losses
        {
            get => _losses;
            set => _losses = value;
        }

        /// <summary>
        /// Losses readable text.
        /// </summary>
        public string LossesText
        {
            get { return String.Format("{0}L", Losses); }
        }

        /// <summary>
        /// The team ties.
        /// </summary>
        public int Ties
        {
            get => _ties;
            set => _ties = value;
        }

        /// <summary>
        /// Ties readable text.
        /// </summary>
        public string TiesText
        {
            get { return String.Format("{0}T", Ties); }
        }

        /// <summary>
        /// The team points.
        /// </summary>
        public double Points
        {
            get => (double)Wins + (0.5 * Ties);
        }

        /// <summary>
        /// Points readable text.
        /// </summary>
        public string PointsText
        {
            get { return String.Format("{0}Pts", Points); }
        }

        /// <summary>
        /// The team is in a playoff positioning.
        /// </summary>
        public bool Playoff
        {
            get => _playoff;
            set => _playoff = value;
        }

        /// <summary>
        /// The team rank.
        /// </summary>
        public int Rank
        {
            get => _rank;
            set => _rank = value;
        }
    }
}

