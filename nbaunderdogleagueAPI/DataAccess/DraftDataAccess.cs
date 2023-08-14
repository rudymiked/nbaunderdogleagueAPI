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
        string DraftTeam(User user);
        List<DraftEntity> SetupDraft(SetupDraftRequest setupDraftRequest);
        List<UserEntity> DraftedTeams(string groupId);
        List<DraftEntity> GetDraft(string groupId);
        List<TeamEntity> GetAvailableTeamsToDraft(string groupId);
        List<DraftResults> GetDraftResults(string groupId);
    }
    public class DraftDataAccess : IDraftDataAccess
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly IGroupService _groupService;
        private readonly ITableStorageHelper _tableStorageHelper;
        public DraftDataAccess(IOptions<AppConfig> options, ILogger<DraftDataAccess> logger, IUserService userService, ITeamService teamService, IGroupService groupService, ITableStorageHelper tableStorageHelper)
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

        public List<DraftEntity> SetupDraft(SetupDraftRequest setupDraftRequest)
        {
            try {
                // 0. need to check if the draft has already happened,
                //      if so, exit

                List<DraftEntity> draftEntities = GetDraft(setupDraftRequest.GroupId);

                if (draftEntities.Count > 0) {
                    DateTimeOffset draftStartTime = draftEntities.Select(draft => draft.UserStartTime).Min();

                    if (draftStartTime < DateTimeOffset.UtcNow) {
                        _logger.LogError(AppConstants.DraftStarted + " : " + setupDraftRequest.GroupId);
                        return new List<DraftEntity>();
                    }
                }

                // 1. query group, if it doesn't exist, return empty list
                GroupEntity groupEntity = _groupService.GetGroup(setupDraftRequest.GroupId);

                if (groupEntity.Id.ToString() == string.Empty) {
                    // No group Found
                    _logger.LogError(AppConstants.GroupNotFound + " : " + setupDraftRequest.GroupId);
                    return new List<DraftEntity>();
                }

                if (groupEntity.Owner != setupDraftRequest.Email && setupDraftRequest.Email != AppConstants.AdminEmail) {
                    // User does not own this group
                    // User is not admin
                    _logger.LogError(AppConstants.NotOwner + " : " + setupDraftRequest.GroupId);
                    return new List<DraftEntity>();
                }

                // 2. get all users from group

                List<UserEntity> userEntities = _userService.GetUsers(setupDraftRequest.GroupId);

                if (!userEntities.Any()) {
                    // no users found in group
                    // should be at least 1 (owner)
                    _logger.LogError(AppConstants.GroupNoUsersFound + setupDraftRequest.GroupId);

                    return new List<DraftEntity>();
                }

                // 3. set in random draft order
                Random rnd = new();

                var shuffledList = Enumerable.Range(1, userEntities.Count).OrderBy(a => rnd.Next()).ToList();

                draftEntities = new(); // clear draft entities object

                // need to ensure that draft for this group doesn't already exist
                // if it does, this method should just re-shuffle the order and pick up new members

                List<DraftEntity> groupDraft = GetDraft(setupDraftRequest.GroupId);

                Guid draftID = groupDraft.Any() ? groupDraft[0].GroupId : Guid.NewGuid();

                DateTimeOffset utcNow = DateTimeOffset.UtcNow;

                DateTimeOffset draftStart = new DateTimeOffset(
                                                setupDraftRequest.DraftStartDateTime.Year,
                                                setupDraftRequest.DraftStartDateTime.Month,
                                                setupDraftRequest.DraftStartDateTime.Day,
                                                setupDraftRequest.DraftStartDateTime.Hour,
                                                0,
                                                0,
                                                utcNow.Offset);

                for (int i = 0; i < userEntities.Count; i++) {
                    int userStartMinute = setupDraftRequest.DraftStartDateTime.Minute + (setupDraftRequest.DraftWindow * (shuffledList[i] - 1)); // "-1" so first starts at minute :00

                    DateTimeOffset userDraftStart = draftStart.AddMinutes(userStartMinute);
                    DateTimeOffset userDraftEnd = userDraftStart.AddMinutes(setupDraftRequest.DraftWindow);

                    draftEntities.Add(new DraftEntity() {
                        PartitionKey = setupDraftRequest.GroupId,
                        RowKey = userEntities[i].Email,
                        GroupId = Guid.Parse(setupDraftRequest.GroupId),
                        Id = draftID,
                        DraftOrder = shuffledList[i],
                        UserStartTime = userDraftStart,
                        UserEndTime = userDraftEnd,
                        Email = userEntities[i].Email,
                        ETag = ETag.All,
                        Timestamp = utcNow
                    });
                }

                if (setupDraftRequest.ClearTeams) {
                    // if users have already drafted, remove their picks
                    userEntities.Where(user => !string.IsNullOrEmpty(user.Team)).ToList().ForEach(user => user.Team = "");

                    var userResponse = _tableStorageHelper.UpsertEntities(userEntities, AppConstants.UsersTable).Result;

                    if (userResponse == null) {
                        _logger.LogError(AppConstants.UsersCouldNotBeUpdated + setupDraftRequest.GroupId);
                    }
                }

                var draftResponse = _tableStorageHelper.UpsertEntities(draftEntities, AppConstants.DraftTable).Result;

                return (draftResponse == AppConstants.Success) ? draftEntities : new List<DraftEntity>();
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new List<DraftEntity>();
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

            // is the user in this draft?
            DraftEntity userInDraft = userDraft.Where(draft => draft.Email == user.Email).First();

            if (userInDraft == null) {
                return AppConstants.UserNotInDraft;
            }

            // need to get the UserEntity, to ensure the team hasn't already been drafted
            string groupFilter = TableClient.CreateQueryFilter<UserEntity>((userEntity) => userEntity.Group == Guid.Parse(user.Group));
            List<UserEntity> usersInGroup = _tableStorageHelper.QueryEntities<UserEntity>(AppConstants.UsersTable, groupFilter).Result.ToList();

            // No Users in group
            if (usersInGroup.Count == 0) {
                return AppConstants.GroupNoUsersFound + user.Group;
            }

            // 1. Is it the user's turn?
            string usersTurnToDraftResult = UsersTurnToDraft(userInDraft, userDraft, usersInGroup);

            if (usersTurnToDraftResult != AppConstants.Success) {
                // It's not the user's turn
                return usersTurnToDraftResult;
            }

            // 2. Has the user already drafted?
            UserEntity userEntity = usersInGroup.FirstOrDefault((userRow) => userRow.Email == user.Email);

            if (userEntity != null) {
                if (!string.IsNullOrEmpty(userEntity.Team)) {
                    return AppConstants.UserAlreadyDrafted + " " + userEntity.Team;
                }
            } else {
                return AppConstants.UserNotFound;
            }


            // 3. someone already drafted this team
            UserEntity draftedUser = usersInGroup.FirstOrDefault(usersInGroup => usersInGroup.Team == user.Team);

            if (draftedUser != null) {
                return AppConstants.TeamAlreadyDrafted + draftedUser.Email;
            }

            return string.Empty;
        }

        private string UsersTurnToDraft(DraftEntity userDraftData, List<DraftEntity> draft, List<UserEntity> usersInGroup)
        {
            try {

                // collect draft start and end time
                DateTimeOffset draftStartDateTime = draft.OrderBy(draftEntity => draftEntity.UserStartTime).FirstOrDefault().UserStartTime;
                DateTimeOffset draftEndDateTime = draft.OrderByDescending(draftEntity => draftEntity.UserStartTime).FirstOrDefault().UserEndTime;


                DateTimeOffset utcNow = DateTimeOffset.UtcNow;
                DateTimeOffset draftStartTime = new DateTimeOffset(
                                                    draftStartDateTime.Year,
                                                    draftStartDateTime.Month,
                                                    draftStartDateTime.Day,
                                                    draftStartDateTime.Hour,
                                                    draftStartDateTime.Minute,
                                                    0,
                                                    utcNow.Offset);

                // draft has not begun
                if (utcNow < draftStartTime) {
                    return AppConstants.DraftNotStarted;
                }

                // current draft order value

                int lastOrderToDraft = 0;
                int nextUpToDraftOrder = 0;

                // need to collect the lowest draft order of a user that has an empty/null team value
                foreach (UserEntity user in usersInGroup) {
                    if (!string.IsNullOrEmpty(user.Team)) {
                        int userWhoHasntDraftedOrder = draft.FirstOrDefault((d) => d.Email == user.Email).DraftOrder;
                        lastOrderToDraft = Math.Max(lastOrderToDraft, userWhoHasntDraftedOrder);
                    }
                }

                nextUpToDraftOrder = lastOrderToDraft + 1;

                DateTimeOffset userWindowStart = userDraftData.UserStartTime;
                DateTimeOffset userTurnOver = userDraftData.UserEndTime;

                // user missed their turn
                if (utcNow > userTurnOver) {
                    return AppConstants.DraftMissedTurn;
                }

                // it's possible that players drafted early
                // if nextUpToDraftOrder == userDraftData.DraftOrder => they can draft!
                if (nextUpToDraftOrder != userDraftData.DraftOrder && utcNow < userWindowStart) {
                    return AppConstants.PleaseWaitToDraft;
                }

                return AppConstants.Success;
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return AppConstants.SomethingWentWrong;
        }

        public List<DraftEntity> GetDraft(string groupId)
        {
            if (Guid.TryParse(groupId, out Guid groupGruid)) {
                string groupFilter = TableClient.CreateQueryFilter<DraftEntity>((draft) => draft.GroupId == groupGruid);

                return _tableStorageHelper.QueryEntities<DraftEntity>(AppConstants.DraftTable, groupFilter).Result.ToList();
            } else {
                _logger.LogError("Group Id: " + groupId + " is not valid.");
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

        public List<DraftResults> GetDraftResults(string groupId)
        {
            try {
                // 1. Query users in group

                List<UserEntity> users = _userService.GetUsers(groupId);

                // 2. Query draft data for group

                List<DraftEntity> draft = GetDraft(groupId);

                // 3. Query team information

                List<TeamEntity> teamEntities = _teamService.GetTeams();

                // 4. Combine
                List<DraftResults> draftResults = new();

                foreach (DraftEntity d in draft) {
                    UserEntity userEntity = users.FirstOrDefault(user => user.Email == d.Email);
                    TeamEntity teamEntity = teamEntities.FirstOrDefault(team => userEntity?.Team == team.Name);

                    draftResults.Add(new DraftResults() {
                        Id = d.Id,
                        GroupId = Guid.Parse(groupId),
                        Email = d.Email,
                        User = string.IsNullOrEmpty(userEntity.Username) ? d.Email.Split('@')[0] : userEntity.Username,
                        DraftOrder = d.DraftOrder,
                        UserStartTime = d.UserStartTime,
                        UserEndTime = d.UserEndTime,
                        TeamID = teamEntity?.ID,
                        TeamName = teamEntity?.Name,
                        TeamCity = teamEntity?.City,
                    });
                }

                return draftResults;
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new List<DraftResults>();
        }
    }
}
