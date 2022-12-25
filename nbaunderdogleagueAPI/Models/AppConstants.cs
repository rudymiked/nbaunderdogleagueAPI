namespace nbaunderdogleagueAPI.Models
{
    public class AppConstants
    {
        public const string NBAFantasyConfig = "NBAFantasyConfig";
        public const string NBAFantasyConfigLocal = "ConnectionStrings:NBAFantasyConfig";
        public const string NBAAppInsights = "NBAAppInsights";
        public const string APIAppId = "apiAppId";
        public const string Tenant = "Tenant";
        public const string TeamsTable = "Teams";
        public const string UsersTable = "Users";
        public const string ArchiveTable = "Archive";
        public const string GroupsTable = "Groups";
        public const string DraftTable = "Drafts";
        public const string SystemConfigurationTable = "SystemConfiguration";
        public const string ScoreboardTable = "Scoreboard";
        public const string SysConfig_RapidAPITimeout = "RapidAPI_Timeout";
        public const string ManualTeamStats = "ManualTeamStats";
        public const string AppName = "NBAUnderdogLeague";
        public const string ApiUrl = "https://nbaunderdogleagueapi.azurewebsites.net";
        public const string AdminEmail = "rudymiked@gmail.com";
        public const string CurrentNBAStandingsJSON = "https://data.nba.net/prod/v1/current/standings_all.json";

        // Results
        public const string Success = "Success";
        public const string SomethingWentWrong = "Something Went Wrong";

        // Group Results
        public const string GroupNotFound = "Group cannot be found: ";
        public const string GroupNoUsersFound = "No users found in group: ";
        public const string UserAlreadyInGroup = "User already in group: ";
        public const string JoinGroupError = "Something went wrong joining group: ";
        public const string LeaveGroupError = "Something went wrong leaving group: ";
        public const string UserNotFound = "User not found.";
        public const string NotOwner = "User not owner of this group.";
        public const string EmptyTeamStats = "TeamStats data is empty.";

        // User 
        public const string UsersCouldNotBeUpdated = "Could not update user information group: ";

        // Draft 
        public const string DraftNotStarted = "Draft has not started.";
        public const string DraftStarted = "Draft has started.";
        public const string DraftMissedTurn = "You missed your turn to draft.";
        public const string PleaseWaitToDraft = "It's not your turn to draft, please wait.";
        public const string UserNotInDraft = "Could not find user in draft.";
        public const string EmptyDraft = "Draft is empty.";
        public const string MultipleUser = "Multiple instance of a user in group.";
        public const string UserAlreadyDrafted = "User already drafted.";
        public const string TeamAlreadyDrafted = "Team has already been drafted by: ";

        // Ints
        public const int MinYear = 2022; // first year of the app

        // Groups
        public static readonly Guid Group_2022 = Guid.Parse("cee48957-9221-46b7-a80d-ba21b6ccb303");
        public static readonly Guid Group_2021 = Guid.Parse("b7c38436-42b8-4deb-b79d-14d4d5dc5533");
        public static readonly Guid Group_2020 = Guid.Parse("94c57d13-db05-4065-aaf9-8bd5eb81b2d6");
        public static readonly Guid Group_2019 = Guid.Parse("958c0634-715d-452e-b8a2-06c02fd8ac30");
        public static readonly Guid Group_2018 = Guid.Parse("a8b1d131-58ef-422f-ad10-9ad145af9564");

        // Policys
        public const string DefaultAuthPolicy = "DefaultAuthPolicy";
    }
}