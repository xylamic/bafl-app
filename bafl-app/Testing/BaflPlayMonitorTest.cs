#if DEBUG

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
        EmptyTeamMonitor();
        EmptyPlayerMonitor();
        SimplePlayerMonitor();
        SimplePlayerMonitor2();
        SimplePlayerMonitor3();
        SimplePlayerMonitor4();
        NotPlayingPlayerMonitor();
        PlayerMonitorPlayCount();
        PlayerMonitorPlayCountHalf();
        SimpleTeamMonitor();
        System.Console.WriteLine("Completed testing PlayMonitor.");
    }

    public void EmptyPlayerMonitor()
    {
        PrintMethodExecution();

        BaflPlayerMonitor monitor = new BaflPlayerMonitor();

        CAssert(monitor != null);
        CAssert(!monitor.IsPeewee);
        CAssert(monitor.Name == "");
        CAssert(monitor.Number == 0);
        CAssert(monitor.Plays == 0);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NoPlays);
        CAssert(monitor.HalfPlays == false);
        CAssert(monitor.OnField == false);
        CAssert(monitor.IsPlaying == true);
    }

    public void SimplePlayerMonitor()
    {
        PrintMethodExecution();

        BaflPlayerMonitor monitor = new BaflPlayerMonitor();
        monitor.Name = "John Doe";
        monitor.Number = 12;
        monitor.Plays = 5;
        monitor.HalfPlays = true;
        monitor.OnField = true;
        monitor.IsPlaying = true;

        CAssert(monitor != null);
        CAssert(!monitor.IsPeewee);
        CAssert(monitor.Name == "John Doe");
        CAssert(monitor.Number == 12);
        CAssert(monitor.Plays == 5);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays);
        CAssert(monitor.HalfPlays == true);
        CAssert(monitor.OnField == true);
        CAssert(monitor.IsPlaying == true);
    }

    public void SimplePlayerMonitor2()
    {
        PrintMethodExecution();

        BaflPlayerMonitor monitor = new BaflPlayerMonitor(false, 12, "John Doe", 5, true, true, true);

        CAssert(monitor != null);
        CAssert(!monitor.IsPeewee);
        CAssert(monitor.Name == "John Doe");
        CAssert(monitor.Number == 12);
        CAssert(monitor.Plays == 5);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays);
        CAssert(monitor.HalfPlays == true);
        CAssert(monitor.OnField == true);
        CAssert(monitor.IsPlaying == true);
    }

    public void SimplePlayerMonitor3()
    {
        PrintMethodExecution();

        BaflPlayerMonitor monitor = new BaflPlayerMonitor(true, 12, "John Doe", 5, true, false, true);

        CAssert(monitor != null);
        CAssert(monitor.IsPeewee);
        CAssert(monitor.Name == "John Doe");
        CAssert(monitor.Number == 12);
        CAssert(monitor.Plays == 4);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.CompletedPlays);
        CAssert(monitor.HalfPlays == true);
        CAssert(monitor.OnField == false);
        CAssert(monitor.IsPlaying == true);
    }

    public void SimplePlayerMonitor4()
    {
        PrintMethodExecution();

        BaflPlayerMonitor monitor = new BaflPlayerMonitor(true, 12, "", 5, false, false, true);

        CAssert(monitor != null);
        CAssert(monitor.IsPeewee);
        CAssert(monitor.Name == "");
        CAssert(monitor.Number == 12);
        CAssert(monitor.Plays == 5);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays);
        CAssert(monitor.HalfPlays == false);
        CAssert(monitor.OnField == false);
        CAssert(monitor.IsPlaying == true);
    }

    public void PlayerMonitorPlayCount()
    {
        PrintMethodExecution();

        BaflPlayerMonitor monitor = new BaflPlayerMonitor(false, 12, "", 0, false, false, true);

        CAssert(monitor.Plays == 0);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NoPlays);

        for (int i = 0; i < 5; i++)
        {
            monitor.AddPlay();
        }
        CAssert(monitor.Plays == 5);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays);

        for (int i = 0; i < 5; i++) monitor.AddPlay();
        CAssert(monitor.Plays == 10);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays);

        for (int i = 0; i < 5; i++) monitor.AddPlay();
        CAssert(monitor.Plays == 12);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.CompletedPlays);

        for (int i = 0; i < 5; i++) monitor.RemovePlay();
        CAssert(monitor.Plays == 10);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays);

        monitor.AddPlay(); monitor.AddPlay();
        CAssert(monitor.Plays == 12);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.CompletedPlays);
    }

    public void PlayerMonitorPlayCountHalf()
    {
        PrintMethodExecution();

        BaflPlayerMonitor monitor = new BaflPlayerMonitor(false, 15, "", 0, false, false, true);
        monitor.HalfPlays = true;

        CAssert(monitor.Plays == 0);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NoPlays);

        for (int i = 0; i < 5; i++)
        {
            monitor.AddPlay();
        }
        CAssert(monitor.Plays == 5);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays);

        monitor.AddPlay();
        CAssert(monitor.Plays == 6);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.CompletedPlays);

        monitor.RemovePlay();
        CAssert(monitor.Plays == 5);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays);

        monitor.AddPlay();
        CAssert(monitor.Plays == 6);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.CompletedPlays);
    }

    public void NotPlayingPlayerMonitor()
    {
        PrintMethodExecution();

        BaflPlayerMonitor monitor = new BaflPlayerMonitor(true, 12, "", 0, false, false, false);

        CAssert(monitor != null);
        CAssert(monitor.IsPeewee);
        CAssert(monitor.Name == "");
        CAssert(monitor.Number == 12);
        CAssert(monitor.Plays == 0);
        CAssert(monitor.PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NotPlaying);
        CAssert(monitor.HalfPlays == false);
        CAssert(monitor.OnField == false);
        CAssert(monitor.IsPlaying == false);
    }

    public void EmptyTeamMonitor()
    {
        PrintMethodExecution();

        BaflTeamMonitor monitor = new BaflTeamMonitor();

        CAssert(monitor != null);
        CAssert(monitor.ThisTeam == "");
        CAssert(monitor.OpposingTeam == "");
        CAssert(monitor.PlayCount == 0);
        CAssert(monitor.Players.Count == 0);
    }

    public void SimpleTeamMonitor()
    {
        PrintMethodExecution();

        // generate 5 players
        BaflPlayerMonitor[] players = new BaflPlayerMonitor[5];
        for (int i = 0; i < 5; i++)
        {
            players[i] = new BaflPlayerMonitor(false, i, $"Player {i}", 0, true, true, true);
        }

        BaflTeamMonitor monitor = new BaflTeamMonitor(players, "Bay Area", "Hitchcock", 0);

        CAssert(monitor != null);
        CAssert(monitor.ThisTeam == "Bay Area");
        CAssert(monitor.OpposingTeam == "Hitchcock");
        CAssert(monitor.PlayCount == 0);
        CAssert(monitor.PlayerCount == 5);

        foreach (BaflPlayerMonitor player in monitor.Players)
        {
            
        }
    }
}

#endif