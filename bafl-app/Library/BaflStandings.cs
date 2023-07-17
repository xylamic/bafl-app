using System;
namespace bafl_app.library
{
    /// <summary>
    /// The BAFL game calendar.
    /// </summary>
    public class BaflStandings
    {
        private string _title;
        private string _message;
        private List<BaflStandingEntry> _standings;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflStandings()
        {
            _title = "";
            _message = "";
            _standings = new List<BaflStandingEntry>();
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        /// <param name="title">The title of the document.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="standings">The standings.</param>
        public BaflStandings(string title, string message, List<BaflStandingEntry> standings)
        {
            _title = title;
            _message = message;
            _standings = standings;
        }

        /// <summary>
        /// The document title.
        /// </summary>
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        /// <summary>
        /// Get the message.
        /// </summary>
        public string Message
        {
            get => _message;
            set => _message = value;
        }

        /// <summary>
        /// The list of weeks.
        /// </summary>
        public List<BaflStandingEntry> Standings
        {
            get => _standings;
            set => _standings = value;
        }
    }
}

