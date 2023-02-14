using System.Text.Json;
using bafl.library;

namespace bafl_app;

/// <summary>
/// The App.
/// </summary>
public partial class App : Application
{
	// cache file names
	private static string TEAM_FILE_NAME = "team-list.json";
    private static string BOARD_FILE_NAME = "board-list.json";
    private static string SCHEDULE_FILE_NAME = "schedule.json";

    /// <summary>
    /// Has the App been initiated.
    /// </summary>
    private static bool _isInitiatied = false;

    /// <summary>
    /// Construct the app.
    /// </summary>
    public App()
	{
		InitializeComponent();

        // Create the initial placeholder lists
        ClubList = new Dictionary<int, BaflClub>();
        BoardMemberList = new List<BaflBoardMember>();
        ScheduleList = new List<BaflScheduleItem>();

		MainPage = new AppShell();
    }

    /// <summary>
    /// The list of BAFL clubs.
    /// </summary>
    public static Dictionary<int, BaflClub> ClubList { get; private set; }

    /// <summary>
    /// The list of BAFL Board members.
    /// </summary>
    public static List<BaflBoardMember> BoardMemberList { get; private set; }

    /// <summary>
    /// The current schedule.
    /// </summary>
    public static List<BaflScheduleItem> ScheduleList { get; private set; }

    /// <summary>
    /// Update the application configuration.
    /// </summary>
    /// <returns>The async task.</returns>
    public static async Task UpdateConfiguration()
    {
        if (_isInitiatied)
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
            // try to read the latest configuration information.
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

        _isInitiatied = true;
    }

    /// <summary>
    /// This preloads any cache in the case that this is the first time the app
    /// has been run.
    /// </summary>
    /// <returns>Async task.</returns>
    private static async Task PreloadCacheCheck()
    {
        string cacheDir = FileSystem.Current.CacheDirectory;

        // get the cache file locations
        string team_cache_filename = cacheDir + "/" + TEAM_FILE_NAME;
        string board_cache_filename = cacheDir + "/" + BOARD_FILE_NAME;
        string schedule_cache_filename = cacheDir + "/" + SCHEDULE_FILE_NAME;

        // verify the team cache file exists
        if (!File.Exists(team_cache_filename))
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Teams.json");
            using StreamReader reader = new StreamReader(fileStream);
            string rawContent = await reader.ReadToEndAsync();

            await File.WriteAllTextAsync(team_cache_filename, rawContent);

            Console.WriteLine("Loaded raw asset team data.");
        }

        // verify the board cache file exists
        if (!File.Exists(board_cache_filename))
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Board.json");
            using StreamReader reader = new StreamReader(fileStream);
            string rawContent = await reader.ReadToEndAsync();

            await File.WriteAllTextAsync(board_cache_filename, rawContent);

            Console.WriteLine("Loaded raw asset board data.");
        }

        // verify the schedule cache file exists
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

