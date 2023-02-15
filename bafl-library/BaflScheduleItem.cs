using System;
namespace bafl.library
{
    public class BaflScheduleItem
    {
        private DateTime _date;
        private string _name;
        private string _location;
        private bool _notable;

        public BaflScheduleItem()
        {
            _notable = false;
        }

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

        public string DateString
        {
            get { return Date.ToLongDateString(); }
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Location
        {
            get => _location;
            set => _location = value;
        }

        public string LocationString
        {
            get { return String.Format("Location: {0}", Location); }
        }

        public bool Notable
        {
            get => _notable;
            set => _notable = value;
        }
    }
}

