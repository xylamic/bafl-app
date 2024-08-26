#if IS_RI_NET80

namespace bafl_app.testing;

using bafl_app.library;
using System.Text.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class TestClass
{
    public static void Main(string[] args)
    {
        System.Console.WriteLine("Beginning manual tests...");
        
        System.Console.WriteLine("\nTesting BaflBoardMember...");
        BaflBoardMemberTest test = new BaflBoardMemberTest();
        test.BoardGeneration();
        System.Console.WriteLine("\n");

        System.Console.WriteLine("Testing BaflClub...");
        BaflClubTest clubTest = new BaflClubTest();
        clubTest.ClubGeneration();
        System.Console.WriteLine("\n");

        PlayMonitorTest monitorTest = new PlayMonitorTest();
        monitorTest.ExecuteAllTests();
        System.Console.WriteLine("\n");

        System.Console.WriteLine("Completed manual test run.");
    }
}

public class TestBase
{
    public static void PrintMethodExecution([CallerMemberName] string methodName = "")
    {
        Console.WriteLine($"\tExecuting method: {methodName}");
    }
}

#endif