using System;
namespace bafl_app.library
{
    /// <summary>
    /// The BAFL game calendar.
    /// </summary>
    public class BaflStandingEntry
    {
        private string _level;
        private List<BaflStandingTeam> _teams;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflStandingEntry()
        {
            _level = "";
            _teams = new List<BaflStandingTeam>();
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        /// <param name="level">The level of standings.</param>
        /// <param name="teams">The team standings.</param>
        public BaflStandingEntry(string level, List<BaflStandingTeam> teams)
        {
            _level = level;
            _teams = teams;
        }

        /// <summary>
        /// The standings level.
        /// </summary>
        public string Level
        {
            get => _level;
            set => _level = value;
        }

        /// <summary>
        /// The team standings
        /// </summary>
        public List<BaflStandingTeam> Teams
        {
            get => _teams;
            set => _teams = value;
        }
    }
}

