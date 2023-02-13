using System.Text.Json;
using bafl.library;

namespace bafl_app;

public partial class App : Application
{
	
	private static string TEAM_FILE_NAME = "team-list.json";
    private static string BOARD_FILE_NAME = "board-list.json";
    private static string SCHEDULE_FILE_NAME = "schedule.json";

    public App()
	{
		InitializeComponent();

        ClubList = new Dictionary<int, BaflClub>();
        BoardMemberList = new List<BaflBoardMember>();
        ScheduleList = new List<BaflScheduleItem>();
		MainPage = new AppShell();
    }

    public static bool IsInitiatied = false;

    public static Dictionary<int, BaflClub> ClubList { get; private set; }

    public static List<BaflBoardMember> BoardMemberList { get; private set; }

    public static List<BaflScheduleItem> ScheduleList { get; private set; }

    public static async Task UpdateConfiguration()
    {
        if (IsInitiatied)
            return;

        // verify cache files are ready
        await PreloadCacheCheck();

        // get the cache file locations
        string cacheDir = FileSystem.Current.CacheDirectory;
        string team_cache_filename = cacheDir + "/" + TEAM_FILE_NAME;
        string board_cache_filename = cacheDir + "/" + BOARD_FILE_NAME;
        string schedule_cache_filename = cacheDir + "/" + SCHEDULE_FILE_NAME;

        try
        {
            using (HttpClient client = new HttpClient())
            {
                string teamContent = await client.GetStringAsync(BaflUtilities.TEAM_URL);
                string boardContent = await client.GetStringAsync(BaflUtilities.BOARD_URL);
                string scheduleContent = await client.GetStringAsync(BaflUtilities.SCHEDULE_URL);

                await File.WriteAllTextAsync(team_cache_filename, teamContent);
                await File.WriteAllTextAsync(board_cache_filename, boardContent);
                await File.WriteAllTextAsync(schedule_cache_filename, scheduleContent);

                Console.WriteLine("Successfully read latest data from the APIs.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(String.Format("Error reading API data: {0}", ex.ToString()));
        }

        try
        {
            // load team data
            string content = await File.ReadAllTextAsync(team_cache_filename);
            ClubList =
                JsonSerializer.Deserialize<Dictionary<int, BaflClub>>(content);
            Console.WriteLine("Loaded team data.");

            // load board data
            content = await File.ReadAllTextAsync(board_cache_filename);
            BoardMemberList =
                JsonSerializer.Deserialize<List<BaflBoardMember>>(content);
            Console.WriteLine("Loaded board data.");

            // load schedule data
            content = await File.ReadAllTextAsync(schedule_cache_filename);
            ScheduleList =
                JsonSerializer.Deserialize<List<BaflScheduleItem>>(content);
            Console.WriteLine("Loaded schedule data.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(String.Format("Failed to load data: {0}", ex.ToString()));
        }

        IsInitiatied = true;
    }

    /// <summary>
    /// This preloads any cache in the case that this is the first time the app
    /// has been run.
    /// </summary>
    /// <returns>Async task.</returns>
    private static async Task PreloadCacheCheck()
    {
        string cacheDir = FileSystem.Current.CacheDirectory;

        string team_cache_filename = cacheDir + "/" + TEAM_FILE_NAME;
        string board_cache_filename = cacheDir + "/" + BOARD_FILE_NAME;
        string schedule_cache_filename = cacheDir + "/" + SCHEDULE_FILE_NAME;

        if (!File.Exists(team_cache_filename))
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Teams.json");
            using StreamReader reader = new StreamReader(fileStream);
            string rawContent = await reader.ReadToEndAsync();

            await File.WriteAllTextAsync(team_cache_filename, rawContent);

            Console.WriteLine("Loaded raw asset team data.");
        }

        if (!File.Exists(board_cache_filename))
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Board.json");
            using StreamReader reader = new StreamReader(fileStream);
            string rawContent = await reader.ReadToEndAsync();

            await File.WriteAllTextAsync(board_cache_filename, rawContent);

            Console.WriteLine("Loaded raw asset board data.");
        }

        if (!File.Exists(schedule_cache_filename))
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Schedule.json");
            using StreamReader reader = new StreamReader(fileStream);
            string rawContent = await reader.ReadToEndAsync();

            await File.WriteAllTextAsync(schedule_cache_filename, rawContent);

            Console.WriteLine("Loaded raw asset schedule data.");
        }
    }
}

