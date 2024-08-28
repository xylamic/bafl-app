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
        CompleteTeamMonitor();
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

        BaflPlayerMonitor monitor = new BaflPlayerMonitor(false, 12, "", 0, false, true, true);

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

        BaflPlayerMonitor monitor = new BaflPlayerMonitor(false, 15, "", 0, false, true, true);
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

    public void CompleteTeamMonitor()
    {
        PrintMethodExecution();

        // generate 5 players
        BaflPlayerMonitor[] players = new BaflPlayerMonitor[9];
        for (int i = 0; i < 5; i++)
        {
            players[i] = new BaflPlayerMonitor(false, i, $"Player {i}", 0, false, true, true);
        }
        for (int i = 5; i < 5 + 3; i++)
        {
            players[i] = new BaflPlayerMonitor(false, i, $"Player {i}", 0, false, false, true);
        }
        players[8] = new BaflPlayerMonitor(false, 8, "Player 8", 0, false, false, false);

        BaflTeamMonitor monitor = new BaflTeamMonitor(players, "Bay Area", "Hitchcock", 0);

        CAssert(monitor != null);
        CAssert(monitor.ThisTeam == "Bay Area");
        CAssert(monitor.OpposingTeam == "Hitchcock");
        CAssert(monitor.PlayCount == 0);
        CAssert(monitor.PlayerCount == 9);

        monitor.RunPlay();
        monitor.RunPlay();

        int[] playCounts = new int[] { 2, 2, 2, 2, 2, 0, 0, 0, 0 };
        for (int i = 0; i < 9; i++)
        {
            CAssert(monitor.Players[i].Plays == playCounts[i]);
        }

        CAssert(monitor.TeamComplete == false);
        monitor.Players[4].OnField = false;
        monitor.Players[5].OnField = true;
        monitor.Players[6].OnField = true;

        monitor.RunPlay();
        monitor.RunPlay();
        playCounts = new int[] { 4, 4, 4, 4, 2, 2, 2, 0, 0 };
        for (int i = 0; i < 9; i++)
        {
            CAssert(monitor.Players[i].Plays == playCounts[i]);
        }

        for (int i = 0; i < 9; i++)
        {
            monitor.RunPlay();
        }
        playCounts = new int[] { 13, 13, 13, 13, 2, 11, 11, 0, 0 };
        for (int i = 0; i < 9; i++)
        {
            if (playCounts[i] > 12)
                CAssert(monitor.Players[i].Plays == 12, $"Player{i}==12");
            else
                CAssert(monitor.Players[i].Plays == playCounts[i], $"Player{i}=={playCounts[i]}");
            if (i < 7)
            {
                if (playCounts[i] >= 12)
                    CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.CompletedPlays);
                else
                    CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays, $"P{i}>{monitor.Players[i].PlayStatus}!=PartialPlays");
            }
            else if (i == 7)
                CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NoPlays);
            else
                CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NotPlaying);
        }

        bool[] onField = new bool[] { false, false, false, true, true, true, true, true, false };
        for (int i = 0; i < 9; i++)
        {
            monitor.Players[i].OnField = onField[i];
        }
        for (int i = 0; i < 12; i++)
        {
            monitor.RunPlay();
        }

        playCounts = new int[] { 13, 13, 13, 25, 14, 23, 23, 12, 0 };
        for (int i = 0; i < 9; i++)
        {
            if (i < 8)
            {
                CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.CompletedPlays);
                if (playCounts[i] > 12)
                    CAssert(monitor.Players[i].Plays == 12, $"Player{i}==12");
                else
                    CAssert(monitor.Players[i].Plays == playCounts[i], $"Player{i}=={playCounts[i]}");
            }
            else
            {
                CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NotPlaying);
                CAssert(monitor.Players[i].Plays == 0);
            }
        }
        CAssert(monitor.TeamComplete == true);

        monitor.UndoPlay();
        CAssert(monitor.TeamComplete == false);
        playCounts = new int[] { 13, 13, 13, 24, 13, 22, 22, 11, 0 };
        for (int i = 0; i < 9; i++)
        {
            if (i < 8)
            {
                
                if (playCounts[i] >= 12)
                {
                    CAssert(monitor.Players[i].Plays == 12, $"Player{i}==12");
                    CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.CompletedPlays);
                }
                else
                {
                    CAssert(monitor.Players[i].Plays == playCounts[i], $"Player{i}=={playCounts[i]}");
                    CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.PartialPlays);
                }
            }
            else
            {
                CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NotPlaying);
                CAssert(monitor.Players[i].Plays == 0);
            }
        }

        monitor.RunPlay();
        CAssert(monitor.TeamComplete == true);
        playCounts = new int[] { 13, 13, 13, 25, 14, 23, 23, 12, 0 };
        for (int i = 0; i < 9; i++)
        {
            if (i < 8)
            {
                CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.CompletedPlays);
                if (playCounts[i] > 12)
                    CAssert(monitor.Players[i].Plays == 12, $"Player{i}==12");
                else
                    CAssert(monitor.Players[i].Plays == playCounts[i], $"Player{i}=={playCounts[i]}");
            }
            else
            {
                CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NotPlaying);
                CAssert(monitor.Players[i].Plays == 0);
            }
        }

        CAssert(monitor.UndoAllowed == true);
        monitor.ResetPlays();
        playCounts = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < 9; i++)
        {
            CAssert(monitor.Players[i].Plays == playCounts[i]);
            if (i < 8)
                CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NoPlays);
            else
                CAssert(monitor.Players[i].PlayStatus == BaflPlayerMonitor.PlayerPlayStatus.NotPlaying);
        }
        CAssert(monitor.TeamComplete == false);
        CAssert(monitor.UndoAllowed == false);
    }
}

#endif