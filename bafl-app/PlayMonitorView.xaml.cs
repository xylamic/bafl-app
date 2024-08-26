namespace bafl_app;

using System;
using bafl_app.library;

/// <summary>
/// The Zelle view.
/// </summary>
public partial class PlayMonitorView : ContentPage
{
    private DateTime _selectedBirthdate = new DateTime(2014, 08, 01);
    private bool _isCheer = false;
    private bool _upLevel = false;

    private string[] _footballLevels = new string[] {
        "TOO YOUNG",
        "Peewee",
        "Freshman",
        "Sophomore",
        "Junior",
        "Senior",
        "AGED OUT"
    };

    private string[] _footballWeights = new string[]
    {
        "N/A",
        "120",
        "140",
        "160",
        "180",
        "200",
        "N/A"
    };

    /// <summary>
    /// Construct the view.
    /// </summary>
	public PlayMonitorView()
	{
        this.BindingContext = this;

        InitializeComponent();
    }

    private void CalculateText()
    {
        DateTime targetDate = new DateTime(2024, 8, 1);

        int age = GetYearsDifference(_selectedBirthdate, targetDate);
        AgeText = age.ToString();

        if (!_isCheer)
        {
            int level = 0;
            if (age < 5)
                level = 0;
            else if (age < 7)
                level = 1;
            else if (age < 9)
                level = 2;
            else if (age < 10)
                level = 3;
            else if (age < 11)
                level = 4;
            else if (age < 13)
                level = 5;
            else
                level = 6;

            if (_upLevel && level < 5 && level > 0)
                level += 1;

            LevelText = _footballLevels[level];
            WeightText = _footballWeights[level];
        }
        else
        {
            if (age < 4)
                LevelText = "TOO YOUNG";
            else if (age < 8 && !_upLevel)
                LevelText = "Mascot";
            else if (age < 14)
                LevelText = "Cheer/Drill";
            else
                LevelText = "AGED OUT";

            WeightText = "N/A";
        }

        OnPropertyChanged(nameof(AgeText));
        OnPropertyChanged(nameof(LevelText));
        OnPropertyChanged(nameof(WeightText));
    }

    private int GetYearsDifference(DateTime start, DateTime end)
    {
        // Calculate initial year difference
        int years = end.Year - start.Year;

        // Adjust the year difference if the end date is before the start date's anniversary in the current year
        if (end.Month < start.Month || (end.Month == start.Month && end.Day < start.Day))
        {
            years--;
        }

        return years;
    }

    public DateTime SelectedBirthDate
    {
        get { return _selectedBirthdate; }
        set
        {
            _selectedBirthdate = value;
            CalculateText();
        }
    }

    public bool IsCheer
    {
        get { return _isCheer; }
        set
        {
            _isCheer = value;
            CalculateText();
        }
    }

    public bool IsUpLevel
    {
        get { return _upLevel; }
        set
        {
            _upLevel = value;
            CalculateText();
        }
    }

    public string WeightText
    {
        get; set;
    }

    public string AgeText
    {
        get; set;
    }

    public string LevelText
    {
        get; set;
    }

    public string AddDescription
    {
        get
        {
            return "For football, the player must be at or under the maximum weight at " +
                "the time of official monitoring. Once the season starts, one additional pound " +
                "per week will be allowed leading up to Week 6. On Week 6, all players will be " +
                "weighed again while wearing all equipment. They will be allowed an additional " +
                "10lbs for the equipment and 5lbs for the 5 weeks that had passed.";
        }
    }
}


