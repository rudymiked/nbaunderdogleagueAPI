using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IDraftDataAccess
    {
        Dictionary<User, string> DraftTeam(User user);
        List<DraftEntity> SetupDraft(string leagueId);
    }
    public class DraftDataAccess : IDraftDataAccess
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly ILeagueService _leagueService;
        private readonly ITableStorageHelper _tableStorageHelper;
        public DraftDataAccess(IOptions<AppConfig> options, ILogger<TeamDataAccess> logger, IUserService userService, ITeamService teamService, ILeagueService leagueService, ITableStorageHelper tableStorageHelper)
        {
            _appConfig = options.Value;
            _logger = logger;
            _userService = userService;
            _teamService = teamService;
            _leagueService = leagueService;
            _tableStorageHelper = tableStorageHelper;
        }

        public Dictionary<User, string> DraftTeam(User user)
        {
            string validation = ValidateUserCanDraftTeam(user);

            // Is user good to draft?
            if (validation == string.Empty) {

                //perform draft
                UserEntity userDraftEntity = new() {
                    PartitionKey = user.League.ToString(),
                    RowKey = user.Email,
                    Email = user.Email,
                    League = user.League,
                    Team = user.Team,
                    ETag = ETag.All,
                    Timestamp = DateTime.Now
                };

                var response = _tableStorageHelper.UpsertEntity(userDraftEntity, AppConstants.UsersTable).Result;

                if (response != null && !response.IsError) {
                    // Successfully Drafted!
                    return new Dictionary<User, string> { { user, AppConstants.Success } };
                } else {
                    // Something went wrong while drafting
                    return new Dictionary<User, string> { { user, AppConstants.SomethingWentWrong } };
                }

            } else {
                // validation failed
                return new Dictionary<User, string> { { user, validation } }; ;
            }
        }

        public List<DraftEntity> SetupDraft(string leagueId)
        {
            // 1. query league, if it doesn't exist, return empty list
            LeagueEntity leagueEntity = _leagueService.GetLeague(leagueId);

            if (leagueEntity.Id.ToString() == string.Empty) {
                // No League Found
                _logger.LogError(AppConstants.LeagueNotFound + " : " + leagueId);
                return new List<DraftEntity>();
            }

            // 2. get all users from league

            List<UserEntity> userEntities = _userService.GetUsers(leagueId);

            if (!userEntities.Any()) {
                // no users found in league
                // should be at least 1 (owner)
                _logger.LogError(AppConstants.LeagueNoUsersFound + leagueId);

                return new List<DraftEntity>();
            }

            // 3. set in random draft order

            int usersInDraft = userEntities.Count;
            Random rnd = new();

            var shuffledList = Enumerable.Range(1, usersInDraft).OrderBy(a => rnd.Next()).ToList();

            List<DraftEntity> draftEntities = new();

            Guid draftID = Guid.NewGuid();

            for (int i = 0; i < usersInDraft; i++) {
                draftEntities.Add(new DraftEntity() {
                    PartitionKey = leagueId,
                    RowKey = draftID.ToString(),
                    LeagueId = Guid.Parse(leagueId),
                    Id = draftID,
                    DraftOrder = shuffledList[i],
                    Email = userEntities[i].Email,
                    ETag = ETag.All,
                    Timestamp = DateTime.Now
                });
            }

            var response = _tableStorageHelper.UpsertEntities(draftEntities, AppConstants.DraftTable).Result;

            return (response != null && !response.GetRawResponse().IsError) ? draftEntities : new List<DraftEntity>();
        }

        private string ValidateUserCanDraftTeam(User user)
        {
            // Validations:
            // *0. Is the user in this draft?
            // *1. Is it the user's turn to draft? 
            //      1.1 Time Now is greater than DraftStartHour and less than DraftStartHour + DraftWindowMinutes
            // *2. Has the user already drafted?
            //      2.1 User email is not already present in Governer column (maybe there is another way to do this?)
            // *3. Is the team available?
            //      3.1 Governer column is BLANK next to team. Could be good to do it this way, since it is one query for #2 and #3

            // Get users from draft
            var draftResponse = _tableStorageHelper.QueryEntities<DraftEntity>(AppConstants.DraftTable).Result;

            if (!draftResponse.Any()) {
                // no users in draft
                return AppConstants.EmptyDraft;
            }

            DraftEntity userInDraft = draftResponse.ToHashSet().Where(draft => draft.Email == user.Email).First();

            if (userInDraft == null) {
                return AppConstants.UserNotInDraft;
            }

            string usersTurnToDraftResult = UsersTurnToDraft(userInDraft.DraftOrder);

            if (usersTurnToDraftResult != AppConstants.Success) {
                // It's not the user's turn
                return usersTurnToDraftResult;
            }

            // create filter for user in specific league
            string leagueFilter = TableClient.CreateQueryFilter<DraftEntity>((draft) => draft.LeagueId == user.League);

            // get all user information for league information
            var usersInLeagueResponse = _tableStorageHelper.QueryEntities<UserEntity>(AppConstants.UsersTable, leagueFilter).Result;

            if (usersInLeagueResponse.ToList().Count == 0) {
                return AppConstants.LeagueNoUsersFound + user.League;
            }

            // someone already drafted this team
            UserEntity draftedUser = usersInLeagueResponse.ToHashSet().Where(usersInLeague => usersInLeague.Team == user.Team).First();

            if (draftedUser != null) {
                return AppConstants.TeamAlreadyDrafted + draftedUser.Email;
            }

            return string.Empty;
        }

        private string UsersTurnToDraft(int order)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime draftStartTime = new(utcNow.Year, _appConfig.DraftStartMonth, _appConfig.DraftStartDay, _appConfig.DraftStartHour, 0, 0);

            // draft has not begun
            if (draftStartTime > utcNow) {
                return AppConstants.DraftNotStarted;
            }

            // user needs to wait their turn
            // order starts at 1, but we want it to start at 0 for this calculation
            DateTime userCanStart = draftStartTime.AddMinutes((order - 1) * _appConfig.DraftWindowMinutes);
            DateTime userTurnOver = draftStartTime.AddMinutes(_appConfig.DraftWindowMinutes);

            if (userCanStart < utcNow) {
                return AppConstants.PleaseWaitToDraft;
            }

            if (utcNow > userTurnOver) {
                return AppConstants.DraftMissedTurn;
            }

            //if (userCanStart > utcNow && utcNow < userTurnOver) {
            return AppConstants.Success;
            //}
        }
    }
}
