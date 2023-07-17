using System;
namespace bafl_app.library
{
    /// <summary>
    /// The BAFL game calendar.
    /// </summary>
    public class BaflGameCalendar
    {
        private string _title;
        private string _message;
        private List<BaflGameWeek> _weeks;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflGameCalendar()
        {
            _title = "";
            _message = "";
            _weeks = new List<BaflGameWeek>();
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        /// <param name="title">The title of the document.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="weeks">The game weeks.</param>
        public BaflGameCalendar(string title, string message, List<BaflGameWeek> weeks)
        {
            _title = title;
            _message = message;
            _weeks = weeks;
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
        public List<BaflGameWeek> Weeks
        {
            get => _weeks;
            set => _weeks = value;
        }
    }
}

