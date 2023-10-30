using System;
using System.Globalization;
using System.Text.Json;
using bafl_app.library;

namespace bafl_app;

/// <summary>
/// Inherited class that customizes for Drill.
/// </summary>
public class DrillCompView: CheerCompView
{
    public DrillCompView()
    {
        _accessUrl = BaflUtilities.DRILLCOMP_URL;
        _mainFilter = "Drill";
    }

    /// <summary>
    /// Get the main text for the form (e.g. Cheer or Drill)
    /// </summary>
    public override string MainText
    {
        get
        {
            if (CheerShown)
                return "⭐️  Drill  ⭐️";
            else
                return "Drill";
        }
    }
}


