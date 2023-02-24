using System;
namespace bafl_app.library
{
    /// <summary>
    /// A timed event in a BAFL event schedule.
    /// </summary>
    public class BaflEventLineItem
    {
        private string _scheduledStart;
        private string _status;
        private string _name;
        private bool _highlight;
        private bool _notable;
        private string _group;

        /// <summary>
        /// Construct the item.
        /// </summary>
        public BaflEventLineItem()
        {
            _scheduledStart = "12:00AM";
            _status = "";
            _name = "";
            _highlight = false;
            _notable = false;
            _group = "";
        }

        /// <summary>
        /// Construct the item.
        /// </summary>
        /// <param name="group">The grouping for the item.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="scheduled">The scheduled start time of the item.</param>
        /// <param name="status">The status string.</param>
        /// <param name="highlight">Whether it should be highlighted.</param>
        /// <param name="notable">Whether it is notable.</param>
        public BaflEventLineItem(
            string group,
            string name,
            string scheduled,
            string status,
            bool highlight,
            bool notable)
        {
            _group = group;
            _scheduledStart = scheduled;
            _name = name;
            _status = status;
            _highlight = highlight;
            _notable = notable;
        }

        /// <summary>
        /// The grouping for the item.
        /// </summary>
        public string Group
        {
            get => _group;
            set => _group = value;
        }

        /// <summary>
        /// The scheduled start time of the item.
        /// </summary>
        public string ScheduledStart
        {
            get => _scheduledStart;
            set => _scheduledStart = value;
        }

        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// The status of the item.
        /// </summary>
        public string Status
        {
            get => _status;
            set => _status = value;
        }

        /// <summary>
        /// Whether to highlight this item.
        /// </summary>
        public bool Highlight
        {
            get => _highlight;
            set => _highlight = value;
        }

        /// <summary>
        /// Whether this item is notable.
        /// </summary>
        public bool Notable
        {
            get => _notable;
            set => _notable = value;
        }
    }
}

