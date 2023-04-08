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
        _viewType = ViewType.Drill;
        _accessUrl = BaflUtilities.DRILLCOMP_URL;
        _accessCode = App.GetApiKey("drillcomp");
        _mainFilter = "Drill";
    }
}


