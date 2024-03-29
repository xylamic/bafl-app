﻿using System.Text.Json;
using bafl_app.library;

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

    // cache file paths
    private static string _team_cache_filename;
    private static string _board_cache_filename;
    private static string _schedule_cache_filename;

    private static BaflAppConfig _config = new BaflAppConfig();

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

        // get the cache file locations
        string cacheDir = FileSystem.Current.CacheDirectory;
        _team_cache_filename = cacheDir + "/" + TEAM_FILE_NAME;
        _board_cache_filename = cacheDir + "/" + BOARD_FILE_NAME;
        _schedule_cache_filename = cacheDir + "/" + SCHEDULE_FILE_NAME;

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
    public static async Task LoadConfiguration()
    {
        if (_isInitiatied)
            return;

        // verify cache files are ready
        await PreloadCacheCheck();

        // get the latest files from the internet
        await PullRemoteConfiguration();

        // load the files
        await LoadCacheFiles();

        _isInitiatied = true;
    }

    /// <summary>
    /// Get the API key.
    /// </summary>
    /// <returns>The key or NULL if not found.</returns>
    public static async Task<string> GetApiKey()
    {
        if (String.IsNullOrEmpty(_config.Key))
        {
            try
            {
                // try to read the latest configuration information.
                using (HttpClient client = new HttpClient())
                {
                    StringContent postContent = new StringContent("{\"context\": \"BaflApp\"}");
                    HttpResponseMessage response = await client.PostAsync(BaflUtilities.CONFIG_URL,
                        postContent);
                    string keyContent = await response.Content.ReadAsStringAsync();
                    _config = JsonSerializer.Deserialize<BaflAppConfig>(keyContent);

                    Console.WriteLine("Successfully read config data from the APIs.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error reading API data: {0}", ex.ToString()));
                return "";
            }
        }

        return _config.Key;
    }

    /// <summary>
    /// Update the configuration from the remote URLs.
    /// </summary>
    /// <returns>The async task.</returns>
    public static async Task PullRemoteConfiguration()
    {
        try
        {
            // try to read the latest configuration information.
            using (HttpClient client = new HttpClient())
            {
                StringContent postContent = new StringContent("{\"context\": \"BaflApp\"}");
                HttpResponseMessage response = await client.PostAsync(BaflUtilities.CONFIG_URL,
                    postContent);
                string keyContent = await response.Content.ReadAsStringAsync();
                _config = JsonSerializer.Deserialize<BaflAppConfig>(keyContent);

                string content = await client.GetStringAsync(String.Format("{0}?code={1}",
                    BaflUtilities.COREINFO_URL, await GetApiKey()));

                Dictionary<string, string> contentDict = JsonSerializer.Deserialize<Dictionary<string, string>>(content);

                await File.WriteAllTextAsync(_team_cache_filename, contentDict["teams"]);
                await File.WriteAllTextAsync(_board_cache_filename, contentDict["board"]);
                await File.WriteAllTextAsync(_schedule_cache_filename, contentDict["schedule"]);

                Console.WriteLine("Successfully read latest data from the APIs.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(String.Format("Error reading API data: {0}", ex.ToString()));
        }
    }

    private static async Task LoadCacheFiles()
    {
        try
        {
            // load team data
            string content = await File.ReadAllTextAsync(_team_cache_filename);
            ClubList =
                JsonSerializer.Deserialize<Dictionary<int, BaflClub>>(content);
            Console.WriteLine("Loaded team data.");

            // load board data
            content = await File.ReadAllTextAsync(_board_cache_filename);
            BoardMemberList =
                JsonSerializer.Deserialize<List<BaflBoardMember>>(content);
            Console.WriteLine("Loaded board data.");

            // load schedule data
            content = await File.ReadAllTextAsync(_schedule_cache_filename);
            ScheduleList =
                JsonSerializer.Deserialize<List<BaflScheduleItem>>(content);
            Console.WriteLine("Loaded schedule data.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(String.Format("Failed to load data: {0}", ex.ToString()));
        }
    }

    /// <summary>
    /// This preloads any cache in the case that this is the first time the app
    /// has been run.
    /// </summary>
    /// <returns>Async task.</returns>
    private static async Task PreloadCacheCheck()
    {
        // verify the team cache file exists
        if (!File.Exists(_team_cache_filename))
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Teams.json");
            using StreamReader reader = new StreamReader(fileStream);
            string rawContent = await reader.ReadToEndAsync();

            await File.WriteAllTextAsync(_team_cache_filename, rawContent);

            Console.WriteLine("Loaded raw asset team data.");
        }

        // verify the board cache file exists
        if (!File.Exists(_board_cache_filename))
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Board.json");
            using StreamReader reader = new StreamReader(fileStream);
            string rawContent = await reader.ReadToEndAsync();

            await File.WriteAllTextAsync(_board_cache_filename, rawContent);

            Console.WriteLine("Loaded raw asset board data.");
        }

        // verify the schedule cache file exists
        if (!File.Exists(_schedule_cache_filename))
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Schedule.json");
            using StreamReader reader = new StreamReader(fileStream);
            string rawContent = await reader.ReadToEndAsync();

            await File.WriteAllTextAsync(_schedule_cache_filename, rawContent);

            Console.WriteLine("Loaded raw asset schedule data.");
        }
    }
}

