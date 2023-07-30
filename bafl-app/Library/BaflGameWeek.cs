using System;
namespace bafl_app.library
{
    /// <summary>
    /// The BAFL game week.
    /// </summary>
    public class BaflGameWeek
    {
        private string _week;
        private DateTime _date;
        private List<BaflGameMatchup> _matchups;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflGameWeek()
        {
            _week = "";
            _date = DateTime.MinValue;
            _matchups = new List<BaflGameMatchup>();
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflGameWeek(string week, DateTime date, List<BaflGameMatchup> matchups)
        {
            _week = week;
            _date = date;
            _matchups = matchups;
        }

        /// <summary>
        /// Pretty text for the week.
        /// </summary>
        public string WeekText
        {
            get {
                return String.Format(
                "{0}, {1}",
                _week,
                _date.ToString("MMMM d"));
            }
        }

        /// <summary>
        /// The matchups week.
        /// </summary>
        public string Week
        {
            get => _week;
            set => _week = value;
        }

        /// <summary>
        /// The date of the matchups.
        /// </summary>
        public DateTime Date
        {
            get => _date;
            set => _date = value;
        }

        /// <summary>
        /// The list of matchups.
        /// </summary>
        public List<BaflGameMatchup> Matchups
        {
            get => _matchups;
            set => _matchups = value;
        }
    }
}

