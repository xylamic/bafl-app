using System;
namespace bafl_app.library
{
    /// <summary>
    /// A BAFL event details.
    /// </summary>
    public class BaflEvent
    {
        private string _name;
        private DateTime _date;
        private string _message;
        private string _doorsOpen;
        private string _moreinfo;
        private string _tickets;
        private List<BaflEventLineItem> _schedule;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflEvent()
        {
            _name = "";
            _date = DateTime.MinValue;
            _message = "";
            _doorsOpen = "";
            _moreinfo = "";
            _tickets = "";
            _schedule = new List<BaflEventLineItem>();
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="date">The date of the event.</param>
        /// <param name="message">Any unique message, if needed.</param>
        /// <param name="doorsOpen">The time the doors open.</param>
        /// <param name="moreInfo">The more info URL link.</param>
        /// <param name="tickets">The tickets URL.</param>
        /// <param name="items">The event schedule.</param>
        public BaflEvent(
            string name,
            DateTime date,
            string message,
            string doorsOpen,
            string moreInfo,
            string tickets,
            BaflEventLineItem[] items)
        {
            _name = name;
            _date = date;
            _message = message;
            _doorsOpen = doorsOpen;
            _moreinfo = moreInfo;
            _schedule = new List<BaflEventLineItem>(items);
        }

        /// <summary>
        /// The date of the event.
        /// </summary>
        public DateTime Date
        {
            get => _date;
            set => _date = value;
        }

        /// <summary>
        /// Get the presentable string for the date.
        /// </summary>
        public string DateString
        {
            get
            {
                if (_date == DateTime.MinValue)
                {
                    return "";
                }
                else
                {
                    return _date.ToLongDateString();
                }
            }
        }

        /// <summary>
        /// The name of the event.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Any unique message for the event.
        /// </summary>
        public string Message
        {
            get => _message;
            set => _message = value;
        }

        /// <summary>
        /// When the doors open.
        /// </summary>
        public string DoorsOpen
        {
            get => _doorsOpen;
            set => _doorsOpen = value;
        }

        /// <summary>
        /// More-info navigation URL.
        /// </summary>
        public string MoreInfo
        {
            get => _moreinfo;
            set => _moreinfo = value;
        }

        /// <summary>
        /// Buy-tickets navigation URL.
        /// </summary>
        public string Tickets
        {
            get => _tickets;
            set => _tickets = value;
        }

        /// <summary>
        /// The list of schedule items.
        /// </summary>
        public List<BaflEventLineItem> Schedule
        {
            get => _schedule;
            set => _schedule = value;
        }
    }
}

