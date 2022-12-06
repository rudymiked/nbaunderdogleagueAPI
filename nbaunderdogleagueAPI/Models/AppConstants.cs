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
        public static readonly string ArchiveTable = "Archive";
        public static readonly string GroupsTable = "Groups";
        public static readonly string DraftTable = "Drafts";
        public static readonly string ScoreboardTable = "Scoreboard";
        public static readonly string ManualTeamStats = "ManualTeamStats";
        public static readonly string AppName = "NBAUnderdogLeague";
        public static readonly string ApiUrl = "https://nbaunderdogleagueapi.azurewebsites.net";
        public static readonly string AdminEmail = "rudymiked@gmail.com";
        public static readonly string CurrentNBAStandingsJSON = "https://data.nba.net/prod/v1/current/standings_all.json";

        // Results
        public static readonly string Success = "Success";
        public static readonly string SomethingWentWrong = "Something Went Wrong";

        // Group Results
        public static readonly string GroupNotFound = "Group cannot be found: ";
        public static readonly string GroupNoUsersFound = "No users found in group: ";
        public static readonly string UserAlreadyInGroup = "User already in group: ";
        public static readonly string JoinGroupError = "Something went wrong joining group: ";
        public static readonly string LeaveGroupError = "Something went wrong leaving group: ";
        public static readonly string UserNotFound = "User not found.";
        public static readonly string NotOwner = "User not owner of this group.";
        public static readonly string EmptyTeamStats = "TeamStats data is empty.";

        // User 
        public static readonly string UsersCouldNotBeUpdated = "Could not update user information group: ";

        // Draft 
        public static readonly string DraftNotStarted = "Draft has not started.";
        public static readonly string DraftStarted = "Draft has started.";
        public static readonly string DraftMissedTurn = "You missed your turn to draft.";
        public static readonly string PleaseWaitToDraft = "It's not your turn to draft, please wait.";
        public static readonly string UserNotInDraft = "Could not find user in draft.";
        public static readonly string EmptyDraft = "Draft is empty.";
        public static readonly string MultipleUser = "Multiple instance of a user in group.";
        public static readonly string UserAlreadyDrafted = "User already drafted.";
        public static readonly string TeamAlreadyDrafted = "Team has already been drafted by: ";

        // Ints
        public static readonly int MinYear = 2022; // first year of the app

        // Groups
        public static readonly Guid Group_2022 = Guid.Parse("cee48957-9221-46b7-a80d-ba21b6ccb303");
        public static readonly Guid Group_2021 = Guid.Parse("b7c38436-42b8-4deb-b79d-14d4d5dc5533");
        public static readonly Guid Group_2020 = Guid.Parse("94c57d13-db05-4065-aaf9-8bd5eb81b2d6");
        public static readonly Guid Group_2019 = Guid.Parse("958c0634-715d-452e-b8a2-06c02fd8ac30");
        public static readonly Guid Group_2018 = Guid.Parse("a8b1d131-58ef-422f-ad10-9ad145af9564");
    }
}