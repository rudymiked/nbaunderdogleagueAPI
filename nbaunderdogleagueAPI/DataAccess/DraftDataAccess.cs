using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IDraftDataAccess
    {
        Dictionary<User, List<string>> DraftTeam(User user);
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

        public Dictionary<User, List<string>> DraftTeam(User user)
        {
            Dictionary<User, List<string>> result = new();

            List<string> validations = ValidateUserCanDraftTeam(user);

            // Is user good to draft?
            if (validations.Count == 0) {

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
                    result.Add(user, validations);
                } else {
                    // Something went wrong while drafting
                    validations.Add(AppConstants.SomethingWentWrong);
                    result.Add(new User(), validations);
                }

            } else {
                result.Add(new User(), validations);
            }

            return result;
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

        private List<string> ValidateUserCanDraftTeam(User user)
        {
            List<string> validations = new();

            // Validations:
            // 1. Is it the user's turn to draft? 
            //      1.1 Time Now is greater than DraftStartHour and less than DraftStartHour + DraftWindowMinutes
            // 2. Has the user already drafted?
            //      2.1 User email is not already present in Governer column (maybe there is another way to do this?)
            // 3. Is the team available?
            //      3.1 Governer column is BLANK next to team. Could be good to do it this way, since it is one query for #2 and #3


            // 1. Is it the user's turn to draft? 
            string userFilter = TableClient.CreateQueryFilter<DraftEntity>((draft) => draft.Email == user.Email && draft.LeagueId == user.League);

            var draftResponse = _tableStorageHelper.QueryEntities<DraftEntity>(AppConstants.DraftTable, userFilter).Result;

            if (!draftResponse.Any()) {
                validations.Add(AppConstants.UserNotInDraft);
            }

            var userResponse = _tableStorageHelper.QueryEntities<UserEntity>(AppConstants.UsersTable, userFilter).Result;

            if (userResponse.ToList().Count > 1) {
                validations.Add(AppConstants.MultipleUser);
            }


            return validations;
        }

        private string UsersTurnToDraft(int order)
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime draftStartTime = new(2022, _appConfig.DraftStartMonth, _appConfig.DraftStartDay, _appConfig.DraftStartHour, 0, 0);

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
