using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IDraftDataAccess
    {
        Dictionary<User, string> DraftTeam(User user);
        List<DraftEntity> SetupDraft(string groupId);
        List<UserEntity> DraftedTeams(string groupId);
    }
    public class DraftDataAccess : IDraftDataAccess
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly IGroupService _groupService;
        private readonly ITableStorageHelper _tableStorageHelper;
        public DraftDataAccess(IOptions<AppConfig> options, ILogger<TeamDataAccess> logger, IUserService userService, ITeamService teamService, IGroupService groupService, ITableStorageHelper tableStorageHelper)
        {
            _appConfig = options.Value;
            _logger = logger;
            _userService = userService;
            _teamService = teamService;
            _groupService = groupService;
            _tableStorageHelper = tableStorageHelper;
        }

        public Dictionary<User, string> DraftTeam(User user)
        {
            string validation = ValidateUserCanDraftTeam(user);

            // Is user good to draft?
            if (validation == string.Empty) {

                User userResult = _userService.UpserUser(user);

                if (string.IsNullOrEmpty(userResult.Team)) {
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

        public List<DraftEntity> SetupDraft(string groupId)
        {
            // 1. query group, if it doesn't exist, return empty list
            GroupEntity groupEntity = _groupService.GetGroup(groupId);

            if (groupEntity.Id.ToString() == string.Empty) {
                // No group Found
                _logger.LogError(AppConstants.GroupNotFound + " : " + groupId);
                return new List<DraftEntity>();
            }

            // 2. get all users from group

            List<UserEntity> userEntities = _userService.GetUsers(groupId);

            if (!userEntities.Any()) {
                // no users found in group
                // should be at least 1 (owner)
                _logger.LogError(AppConstants.GroupNoUsersFound + groupId);

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
                    PartitionKey = groupId,
                    RowKey = draftID.ToString(),
                    groupId = Guid.Parse(groupId),
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

        public List<UserEntity> DraftedTeams(string groupId)
        {
            // query user table for users in this group
            // might be better placed in UserDataAccess? 
            // But it is a draft method... leaving here for now

            List<UserEntity> groupUsers = _userService.GetUsers(groupId);

            // return users who have picked a team
            return groupUsers.Where(user => !string.IsNullOrEmpty(user.Team)).ToList();
        }

        private string ValidateUserCanDraftTeam(User user)
        {
            // Validations:
            // *-1. Test draft? skip validations
            // *0. Is the user in this draft?
            // *1. Is it the user's turn to draft? 
            //      1.1 Time Now is greater than DraftStartHour and less than DraftStartHour + DraftWindowMinutes
            // *2. Has the user already drafted?
            //      2.1 User email is not already present in Governer column (maybe there is another way to do this?)
            // *3. Is the team available?
            //      3.1 Governer column is BLANK next to team. Could be good to do it this way, since it is one query for #2 and #3


            // TEST DRAFT
            if (_appConfig.TestDraft == 1) {
                return string.Empty;
            }

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

            // create filter for user in specific group
            string groupFilter = TableClient.CreateQueryFilter<DraftEntity>((draft) => draft.groupId == user.Group);

            // get all user information for group information
            var usersInGroupResponse = _tableStorageHelper.QueryEntities<UserEntity>(AppConstants.UsersTable, groupFilter).Result;

            if (usersInGroupResponse.ToList().Count == 0) {
                return AppConstants.GroupNoUsersFound + user.Group;
            }

            // someone already drafted this team
            UserEntity draftedUser = usersInGroupResponse.ToHashSet().Where(usersInGroup => usersInGroup.Team == user.Team).First();

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
