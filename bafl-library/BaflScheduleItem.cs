using System;
namespace bafl.library
{
    /// <summary>
    /// An event in the BAFL schedule.
    /// </summary>
    public class BaflScheduleItem
    {
        private DateTime _date;
        private string _name;
        private string _location;
        private bool _notable;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflScheduleItem()
        {
            _notable = false;
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        /// <param name="date">The date of the event.</param>
        /// <param name="name">The name of the event.</param>
        /// <param name="location">The location.</param>
        /// <param name="notable">Whether this is a notable event.</param>
        public BaflScheduleItem(
            DateTime date,
            string name,
            string location,
            bool notable)
        {
            _date = date;
            _name = name;
            _location = location;
            _notable = notable;
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
        /// Is the data for this item in the past?
        /// </summary>
        public bool IsDateInPast
        {
            get
            {
                DateTime dt = DateTime.Now.AddDays(-1);
                if (_date < dt)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The opacity value for this item based on whether
        /// it occurred in the past.
        /// </summary>
        public double OpacityValue
        {
            get
            {
                if (IsDateInPast)
                {
                    return 0.5;
                }
                else
                {
                    return 1.0;
                }
            }
        }

        /// <summary>
        /// Get the presentable string for the date.
        /// </summary>
        public string DateString
        {
            get { return Date.ToLongDateString(); }
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
        /// The location of the event.
        /// </summary>
        public string Location
        {
            get => _location;
            set => _location = value;
        }

        /// <summary>
        /// The presentation string for the location.
        /// </summary>
        public string LocationString
        {
            get { return String.Format("Location: {0}", Location); }
        }

        /// <summary>
        /// Whether this event is notable, or important.
        /// </summary>
        public bool Notable
        {
            get => _notable;
            set => _notable = value;
        }
    }
}

