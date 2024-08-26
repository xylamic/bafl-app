#if IS_RI_NET80

namespace bafl_app.testing;

using bafl_app.library;
using System.Diagnostics;
using System.Text.Json;

public class PlayMonitorTest : TestBase
{

    public PlayMonitorTest()
    {
    }

    public void ExecuteAllTests()
    {
        System.Console.WriteLine("Testing PlayMonitor...");
        EmptyPlayMonitor();
        System.Console.WriteLine("Completed testing PlayMonitor.");
    }

    public void EmptyPlayMonitor()
    {
        PrintMethodExecution();

        BaflTeamMonitor monitor = new BaflTeamMonitor();

        Debug.Assert(monitor != null);
        Debug.Assert(monitor.ThisTeam == "");
        Debug.Assert(monitor.OpposingTeam == "");
        Debug.Assert(monitor.PlayCount == 0);
        Debug.Assert(monitor.Players.Count == 0);
    }
}

#endif