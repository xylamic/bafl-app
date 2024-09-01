#if DEBUG

namespace bafl_app.testing;

using bafl_app.library;
using System.Text.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#if IS_RI_NET80

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

#endif

public class TestBase
{
    public static void PrintMethodExecution([CallerMemberName] string methodName = "")
    {
        Console.WriteLine($"\tExecuting method: {methodName}");
    }

    public static void CAssert(bool condition,[CallerMemberName] string methodName = "", [CallerLineNumber] int lineNumber = 0)
    {
        if (!condition)
        {
            Console.WriteLine($"\tAssertion failed {methodName}:{lineNumber}");
        }
    }
}

#endif