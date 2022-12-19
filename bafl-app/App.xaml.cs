using System.Text.Json;
using bafl.library;

namespace bafl_app;

public partial class App : Application
{
	private static string TEAM_URL = "https://xylamic.github.io/bafl-app/team-list.json";
	private static string TEAM_FILE_NAME = "team-list.json";

    public App()
	{
		InitializeComponent();

        ClubList = new Dictionary<int, BAFLClub>();
        ClubList.Add(1, new BAFLClub("SE", "foosball", "", "", ""));
		MainPage = new AppShell();
    }

    public static bool IsInitiatied = false;

    public static Dictionary<int, BAFLClub> ClubList { get; private set; }

    public static async Task UpdateConfiguration()
    {
        if (IsInitiatied)
            return;

        string cacheDir = FileSystem.Current.CacheDirectory;

        // if the team data does not already exist, then load from assets
        if (!File.Exists(cacheDir + "/" + TEAM_FILE_NAME))
        {
            using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Teams.json");
            using StreamReader reader = new StreamReader(fileStream);
            string rawContent = await reader.ReadToEndAsync();

            await File.WriteAllTextAsync(cacheDir + "/" + TEAM_FILE_NAME, rawContent);

            Console.WriteLine("Loaded raw asset team data.");
        }

        // read the most recent content, in case of web error
        string content = await File.ReadAllTextAsync(cacheDir + "/" + TEAM_FILE_NAME);

        // try to read the latest from the website
        try
        {
            using (HttpClient client = new HttpClient())
            {
                content = await client.GetStringAsync(TEAM_URL);
                await File.WriteAllTextAsync(cacheDir + "/" + TEAM_FILE_NAME, content);
                Console.WriteLine("Successfully read latest team data.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(String.Format("Error reading team data: {0}", ex.ToString()));
        }

        // deserialize the teams
        ClubList =
            JsonSerializer.Deserialize<Dictionary<int, BAFLClub>>(content);
        Console.WriteLine("Loaded team data.");

        IsInitiatied = true;
    }
}

