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

        public static readonly string Msg_PullRefreshTime = "🔽  Updated {0}, pull to refresh  🔽";

        public static readonly string Msg_FailRefreshTime = "🔽  Failed load {0}, try again  🔽";

        public static readonly string Msg_PullRefreshDay = "🔽  Updated {0}  🔽";

        public static readonly string Msg_FailRefreshDay = "🔽  Failed load, try again  🔽";

        /// <summary>
        /// The total number of play required for a freshment to senior player.
        /// </summary>
        public static readonly int TotalPlays_FrSr = 12;

        /// <summary>
        /// The total number of play required for a peewee player.
        /// </summary>
        public static readonly int TotalPlaysPw = 8;
    }
}

