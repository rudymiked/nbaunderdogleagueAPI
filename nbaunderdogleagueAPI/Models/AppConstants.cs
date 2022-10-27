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
        public static readonly string GroupsTable = "Groups";
        public static readonly string DraftTable = "Drafts";
        public static readonly string AppName = "NBAUnderdogLeague";
        public static readonly string ApiUrl = "https://nbaunderdogleagueapi.azurewebsites.net";
        public static readonly string AdminEmail = "rudymiked@gmail.com";

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
        public static readonly string DraftMissedTurn = "You missed your turn to draft.";
        public static readonly string PleaseWaitToDraft = "It's not your turn to draft, please wait.";
        public static readonly string UserNotInDraft = "Could not find user in draft.";
        public static readonly string EmptyDraft = "Draft is empty.";
        public static readonly string MultipleUser = "Multiple instance of a user in group.";
        public static readonly string UserAlreadyDrafted = "User already drafted.";
        public static readonly string TeamAlreadyDrafted = "Team has already been drafted by: ";

        // Ints
        public static readonly int MinYear = 2022; // first year of the app
    }
}