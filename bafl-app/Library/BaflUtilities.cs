using System;
namespace bafl_app.library
{
	public static class BaflUtilities
	{
        public static readonly string CONFIG_URL = "https://baflapp.azurewebsites.net/api/app-config";

        public static readonly string COREINFO_URL = "https://baflapp.azurewebsites.net/api/coreinfo";

        public static readonly string TEAM_URL = "https://baflapp.azurewebsites.net/api/teams";

        public static readonly string BOARD_URL = "https://baflapp.azurewebsites.net/api/board";

        public static readonly string SCHEDULE_URL = "https://baflapp.azurewebsites.net/api/schedule";

        public static readonly string CHEERCOMP_URL = "https://baflapp.azurewebsites.net/api/cheercomp";

        public static readonly string DRILLCOMP_URL = "https://baflapp.azurewebsites.net/api/drillcomp";

        public static readonly string GAMECALENDAR_URL = "https://baflapp.azurewebsites.net/api/calendar";

        public static readonly string GAMECALENDAR9V9_URL = "https://baflapp.azurewebsites.net/api/calendar9v9";

        public static readonly string STANDINGS_URL = "https://baflapp.azurewebsites.net/api/standings";

        public static readonly string STANDINGS9V9_URL = "https://baflapp.azurewebsites.net/api/standings9v9";

        public static readonly string ZelleUrl = "https://enroll.zellepay.com/qr-codes?data=ewogICJuYW1lIiA6ICJCQVkgQVJFQSBGT09UQkFMTCBMRUFHVUUgSU5DIiwKICAidG9rZW4iIDogImJhZmx0cmVhc3VyZXIyMUBnbWFpbC5jb20iLAogICJhY3Rpb24iIDogInBheW1lbnQiCn0=";

        public static readonly string CashAppUrl = "https://cash.app/$payBAFL?qr=1";

        public static readonly string SquareUrl = "https://square.link/u/anPW8szd";

        private static readonly string Msg_PullRefreshTime = "🔽  Updated @ {0}, pull to refresh  🔽";

        private static readonly string Msg_FailRefreshTime = "🔽  Failed load @ {0}, try again. Detail: {1}  🔽";

        private static readonly string Msg_PullRefreshDay = "🔽  Updated @ {0}, pull to refresh  🔽";

        private static readonly string Msg_FailRefreshDay = "🔽  Failed load @ {0}, try again. Detail: {1}  🔽";

        /// <summary>
        /// Generate a refresh update message.
        /// </summary>
        /// <param name="isDay">Whether it's a day message, otherwise a time message.</param>
        /// <returns>The update message to show.</returns>
        public static string GenerateUpdateMessage(bool isDay)
        {
            if (isDay)
            {
                return String.Format(BaflUtilities.Msg_PullRefreshDay, DateTime.Now.ToShortDateString());
            }
            else
            {
                return String.Format(BaflUtilities.Msg_PullRefreshTime, DateTime.Now.ToShortTimeString());
            }
        }

        /// <summary>
        /// Generate an error message for refreshing.
        /// </summary>
        /// <param name="isDay">Whether it's a day message (schedule), otherwise a time message (cheer comp).</param>
        /// <param name="ex">The exception that occurred.</param>
        /// <returns>The string to show.</returns>
        public static string GenerateErrorMessage(bool isDay, Exception ex)
        {
            if (isDay)
            {
                return String.Format(BaflUtilities.Msg_FailRefreshDay, DateTime.Now.ToShortDateString(), ex.Message);
            }
            else
            {
                return String.Format(BaflUtilities.Msg_FailRefreshTime, DateTime.Now.ToShortTimeString(), ex.Message);
            }
        }

        /// <summary>
        /// The total number of play required for a freshment to senior player.
        /// </summary>
        public static readonly int TotalPlays_FrSr = 12;

        /// <summary>
        /// The total number of play required for a peewee player.
        /// </summary>
        public static readonly int TotalPlaysPw = 8;

        /// <summary>
        /// Write out a file to the cache.
        /// </summary>
        /// <param name="fileName">The name of the file to write.</param>
        /// <param name="content">The content to write to the file.</param>
        /// <returns>The file path.</returns>
        public static async Task<string> CreateFileAsync(string fileName, string content)
        {
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllTextAsync(filePath, content);
            return filePath;
        }
    }
}

