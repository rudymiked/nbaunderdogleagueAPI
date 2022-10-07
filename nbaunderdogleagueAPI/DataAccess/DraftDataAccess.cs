using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IDraftDataAccess
    {
        string DraftTeam(User user);
        List<DraftEntity> SetupDraft(string groupId);
        List<UserEntity> DraftedTeams(string groupId);
        List<DraftEntity> GetDraft(string groupId);
        List<TeamEntity> GetAvailableTeamsToDraft(string groupId);
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

        public string DraftTeam(User user)
        {
            string validation = ValidateUserCanDraftTeam(user);

            // Is user good to draft?
            if (validation == string.Empty) {

                User userResult = _userService.UpsertUser(user);

                if (!string.IsNullOrEmpty(userResult.Team)) {
                    // Successfully Drafted!
                    return AppConstants.Success;
                } else {
                    // Something went wrong while drafting
                    return AppConstants.SomethingWentWrong;
                }

            } else {
                // validation failed
                return validation;
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

            // need to ensure that draft for this group doesn't already exist
            // if it does, this method should just re-shuffle the order and pick up new members

            List<DraftEntity> groupDraft = GetDraft(groupId);

            Guid draftID = Guid.NewGuid();

            if (groupDraft.Any()) {
                if (groupDraft.Count == 1) {
                    draftID = groupDraft[0].GroupId;
                } else {
                    _logger.LogError("Groups should only have one draft. Something is wrong");
                }
            }

            DateTimeOffset utcNow = DateTimeOffset.UtcNow;

            for (int i = 0; i < usersInDraft; i++) {
                int draftStartMinute = _appConfig.DraftStartMinute + (_appConfig.DraftWindowMinutes * (shuffledList[i] - 1)); // "-1" so first starts at minute :00

                draftEntities.Add(new DraftEntity() {
                    PartitionKey = groupId,
                    RowKey = userEntities[i].Email,
                    GroupId = Guid.Parse(groupId),
                    Id = draftID,
                    DraftOrder = shuffledList[i],
                    UserStartTime = new DateTime(utcNow.Year, _appConfig.DraftStartMonth, _appConfig.DraftStartDay, _appConfig.DraftStartHour, draftStartMinute, 0),
                    UserEndTime = new DateTime(utcNow.Year, _appConfig.DraftStartMonth, _appConfig.DraftStartDay, _appConfig.DraftStartHour, draftStartMinute + _appConfig.DraftWindowMinutes, 0),
                    Email = userEntities[i].Email,
                    ETag = ETag.All,
                    Timestamp = utcNow
                });
            }

            // if users have already drafted, remove their picks
            userEntities.Where(user => !string.IsNullOrEmpty(user.Team)).ToList().ForEach(user => user.Team = "");

            var userResponse = _tableStorageHelper.UpsertEntities(userEntities, AppConstants.UsersTable).Result;

            if (userResponse == null) {
                _logger.LogError(AppConstants.UsersCouldNotBeUpdated + groupId);
            }

            var draftResponse = _tableStorageHelper.UpsertEntities(draftEntities, AppConstants.DraftTable).Result;

            return (draftResponse != null && !draftResponse.GetRawResponse().IsError) ? draftEntities : new List<DraftEntity>();
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

        private bool IsTestDraft()
        {
            return _appConfig.TestDraft == 1;
        }

        private string ValidateUserCanDraftTeam(User user)
        {
            // Validations:
            // *-1. Test draft? skip validations
            // *0. Is the user in this draft?
            // *1. Is it the user's turn to draft? 
            //      1.1 Time Now is greater than DraftStartHour and less than DraftStartHour + DraftWindowMinutes
            // *2. Has the user already drafted?
            //      2.1 User's team value is not null
            // *3. Is the team available?
            //      3.1 Governer column is BLANK next to team. Could be good to do it this way, since it is one query for #2 and #3


            // -1. TEST DRAFT

            if (IsTestDraft()) return string.Empty;

            // 0. Get users from draft
            List<DraftEntity> userDraft = GetDraft(user.Group.ToString());

            if (!userDraft.Any()) {
                // no users in draft
                return AppConstants.EmptyDraft;
            }

            // need to get the UserEntity, to ensure the team hasn't already been drafted
            string groupFilter = TableClient.CreateQueryFilter<UserEntity>((userEntity) => userEntity.Group == Guid.Parse(user.Group));
            List<UserEntity> usersInGroup = _tableStorageHelper.QueryEntities<UserEntity>(AppConstants.UsersTable, groupFilter).Result.ToList();

            // is the user in this draft?
            DraftEntity userInDraft = userDraft.Where(draft => draft.Email == user.Email).First();

            if (userInDraft == null) {
                return AppConstants.UserNotInDraft;
            }

            // 1. Is it the user's turn?
            string usersTurnToDraftResult = UsersTurnToDraft(userInDraft, userDraft, usersInGroup);

            if (usersTurnToDraftResult != AppConstants.Success) {
                // It's not the user's turn
                return usersTurnToDraftResult;
            }

            if (usersInGroup.Count == 0) {
                return AppConstants.GroupNoUsersFound + user.Group;
            }

            // 3. someone already drafted this team
            UserEntity draftedUser = usersInGroup.Where(usersInGroup => usersInGroup.Team == user.Team).First();

            if (draftedUser != null) {
                return AppConstants.TeamAlreadyDrafted + draftedUser.Email;
            }

            return string.Empty;
        }

        private string UsersTurnToDraft(DraftEntity userDraftData, List<DraftEntity> draft, List<UserEntity> usersInGroup)
        {
            DateTimeOffset utcNow = DateTime.UtcNow;
            DateTimeOffset draftStartTime = new DateTimeOffset(new DateTime(utcNow.Year, _appConfig.DraftStartMonth, _appConfig.DraftStartDay, _appConfig.DraftStartHour, _appConfig.DraftStartMinute, 0), utcNow.Offset);

            // draft has not begun
            if (draftStartTime > utcNow) {
                return AppConstants.DraftNotStarted + " draft starts: " + draftStartTime.ToLocalTime();
            }

            // current draft order value
            List<UserEntity> usersHaventDrafted = usersInGroup.Where(user => string.IsNullOrEmpty(user.Team)).ToList();

            int nextUpToDraftOrder = usersInGroup.Count;

            // need to collect the lowest draft order of a user that has an empty/null team value

            foreach (UserEntity user in usersHaventDrafted) {
                int userOrder = draft.Where(draft => draft.Email == user.Email).FirstOrDefault().DraftOrder;
                nextUpToDraftOrder = Math.Min(nextUpToDraftOrder, userOrder);
            }

            // "userWindowStart" is the beginning of the user's draft window
            // however, they should be able to draft as soon as the user in front of them is done drafting.
            // nextUpToDraftOrder == order <- it's their turn!

            // order starts at 1, but we want it to start at 0 for this calculation
            DateTimeOffset userWindowStart = userDraftData.UserStartTime; // could also use UserStartTime?

            // users will have "draftWindowMinutes" time to draft, starting at their time to start.
            // they can draft anytime before this, as long as their order number is up
            DateTimeOffset userTurnOver = userDraftData.UserEndTime; // could also use UserEndTime?

            if (nextUpToDraftOrder < userDraftData.DraftOrder) {
                return AppConstants.PleaseWaitToDraft + " until " + userWindowStart.ToLocalTime();
            }

            if (utcNow > userTurnOver || nextUpToDraftOrder > userDraftData.DraftOrder) {
                return AppConstants.DraftMissedTurn;
            }

            return AppConstants.Success;
        }

        public List<DraftEntity> GetDraft(string groupId)
        {
            if (Guid.TryParse(groupId, out Guid groupGruid)) {
                string groupFilter = TableClient.CreateQueryFilter<DraftEntity>((draft) => draft.GroupId == groupGruid);

                return _tableStorageHelper.QueryEntities<DraftEntity>(AppConstants.DraftTable, groupFilter).Result.ToList();
            } else {
                return new List<DraftEntity>();
            }
        }

        public List<TeamEntity> GetAvailableTeamsToDraft(string groupId)
        {
            // query every team
            List<TeamEntity> teams = _teamService.GetTeams();

            // query all users

            List<UserEntity> users = _userService.GetUsers(groupId);

            List<TeamEntity> teamsNotDrafted = new();

            // add team information for teams that have not been drafted.
            foreach (TeamEntity team in teams) {
                if (!users.Where(user => user.Team == team.Name).Any()) {
                    teamsNotDrafted.Add(team);
                }
            }

            return teamsNotDrafted;
        }
    }
}
