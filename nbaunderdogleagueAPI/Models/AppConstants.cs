namespace nbaunderdogleagueAPI.Models
{
    public class AppConstants
    {
        public static readonly string NBAFantasyConfig = "NBAFantasyConfig";
        public static readonly string NBAFantasyConfigLocal = "ConnectionStrings:NBAFantasyConfig";
        public static readonly string NBAAppInsights = "NBAAppInsights";
        public static readonly string APIAppId = "apiAppId";
        public static readonly string Tenant = "Tenant";
        public static readonly string TeamsTable = "Teams";
        public static readonly string UsersTable = "Users";
        public static readonly string LeaguesTable = "Leagues";
        public static readonly string AppName = "NBAUnderdogLeague";
        public static readonly string ApiUrl = "https://nbaunderdogleagueapi.azurewebsites.net";
        public static readonly string CurrentNBAStandingsJSON = "https://data.nba.net/prod/v1/current/standings_all.json";

        // Results
        public static readonly string Success = "Success";

        // League Results
        public static readonly string LeagueNotFound = "League cannot be found: ";
        public static readonly string LeagueNoUsersFound = "No Users found in League: ";
        public static readonly string UserAlreadyInLeague = "User already in League: ";
        public static readonly string JoinLeagueError = "Something went wrong joining league: ";
    }
}