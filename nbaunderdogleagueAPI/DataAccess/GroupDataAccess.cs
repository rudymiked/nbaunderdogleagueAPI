using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IGroupDataAccess
    {
        List<GroupStandings> GetGroupStandings(string groupId, int version);
        GroupEntity CreateGroup(string name, string ownerEmail);
        GroupEntity UpsertGroup(GroupEntity group);
        GroupEntity GetGroup(string groupId);
        List<GroupEntity> GetAllGroupsByYear(int year);
        List<GroupEntity> GetAllGroupsUserIsInByYear(string email, int year);
        List<GroupEntity> GetAllGroups();
        string JoinGroup(JoinGroupRequest joinGroupRequest);
        string LeaveGroup(LeaveGroupRequest leaveGroupRequest);
        string ApproveNewGroupMember(ApproveUserRequest approveUserRequest);
        List<JoinGroupRequestEntity> GetJoinGroupRequests(string groupId, string filter = "");
    }
    public class GroupDataAccess : IGroupDataAccess
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly ITableStorageHelper _tableStorageHelper;
        public GroupDataAccess(IOptions<AppConfig> options, ILogger<GroupDataAccess> logger, IUserService userService, ITeamService teamService, ITableStorageHelper tableStorageHelper)
        {
            _appConfig = options.Value;
            _logger = logger;
            _userService = userService;
            _teamService = teamService;
            _tableStorageHelper = tableStorageHelper;
        }

        /*
            GroupStandings:
            version 0: https://stats.nba.com/, DEPRECATED.
            version 1: https://data.nba.net/prod/v1/current/standings_all.json, does not work after deploying to Azure.
            version 2: ManualTeamStats, populated by RapidAPI every 30 mins. 
         */

        public List<GroupStandings> GetGroupStandings(string groupId, int version)
        {
            List<GroupStandings> standings = new();

            // 1. Get Current NBA Standings Data (from NBA stats)
            Dictionary<string, TeamStats> teamStatsDict = _teamService.TeamStatsDictionary(version);

            // something went wrong.
            if (teamStatsDict.Count == 0) {
                _logger.LogError(AppConstants.EmptyTeamStats);
                return new List<GroupStandings>();
            }

            // 2. Get Projected Data (from storage)
            List<TeamEntity> teamsEntities = _teamService.GetTeams();

            // 3. Get Group Info
            GroupEntity group = GetGroup(groupId);

            // 4. Get Users and their teams
            List<UserEntity> userEntities = _userService.GetUsers(groupId);

            if (group.DraftDate > DateTimeOffset.UtcNow || group.DraftDate == DateTimeOffset.MinValue) {
                // draft either hasn't started or hasn't been setup

                foreach (UserEntity user in userEntities) {
                    standings.Add(new GroupStandings {
                        Governor = string.IsNullOrWhiteSpace(user.Username) ? user.Email.Split("@")[0] : user.Username,
                        Email = user.Email,
                        TeamName = "",
                        TeamCity = "",
                        ProjectedWin = 0,
                        ProjectedLoss = 0,
                        Win = 0,
                        Loss = 0,
                        Score = 0,
                        Playoffs = "",
                        PlayoffWins = 0
                    });
                }

                return standings.OrderByDescending(group => group.Governor).ToList();
            }

            int index = 0;
            Dictionary<string, UserEntity> userEntitiesDictionary = userEntities.ToDictionary(user => user.Team ?? (index++).ToString());

            // 5. Combine 1, 2, and 3
            foreach (TeamEntity team in teamsEntities) {
                TeamStats teamStats = teamStatsDict[team.Name];
                userEntitiesDictionary.TryGetValue(team.Name, out UserEntity userEntity);

                if (userEntity != null) {
                    standings.Add(new GroupStandings {
                        Governor = userEntity.Username ?? userEntity.Email.Split("@")[0],
                        Email = userEntity.Email,
                        TeamName = team.Name,
                        TeamCity = team.City,
                        ProjectedWin = team.ProjectedWin,
                        ProjectedLoss = team.ProjectedLoss,
                        Win = teamStats.Wins,
                        Loss = teamStats.Losses,
                        Score = TeamUtils.CalculateTeamScore(team.ProjectedWin, team.ProjectedLoss, teamStats.Wins, teamStats.Losses, teamStats.PlayoffWins),
                        Playoffs = teamStats.ClinchedPlayoffBirth == 1 ? "Yes" : "",
                        PlayoffWins = teamStats.PlayoffWins
                    });
                }
            }

            return standings.OrderByDescending(group => group.Score).ToList();
        }

        public GroupEntity GetGroup(string groupId)
        {
            string filter = TableClient.CreateQueryFilter<GroupEntity>((group) => group.PartitionKey == groupId);

            var response = QueryGroupTable(filter);

            return response.Any() ? response.ToList().FirstOrDefault() : new GroupEntity();
        }

        public List<GroupEntity> GetAllGroupsByYear(int year)
        {
            List<GroupEntity> groupEntities = new();

            string filter = TableClient.CreateQueryFilter<GroupEntity>((group) => group.Year == year);

            var response = QueryGroupTable(filter);

            return response.Any() ? response.ToList() : new List<GroupEntity>();
        }

        public List<GroupEntity> GetAllGroupsUserIsInByYear(string email, int year)
        {
            // Collect all groups that user is in

            string userGroupFilter = TableClient.CreateQueryFilter<UserEntity>((user) => user.Email == email);

            var userRsponse = _tableStorageHelper.QueryEntities<UserEntity>(AppConstants.UsersTable, userGroupFilter).Result;

            if (!userRsponse.Any()) {
                return new List<GroupEntity>();
            }

            var userGroups = userRsponse.ToList().Select(user => user.Group);

            // Filter groups by year 

            string yearFilter = TableClient.CreateQueryFilter<GroupEntity>((group) => group.Year == year);

            var groupResponse = QueryGroupTable(yearFilter);

            List<GroupEntity> groupsByYear = groupResponse.ToList();

            List<Guid> groupsUserIsIn = groupsByYear.Select(group => group.Id).Intersect(userGroups).ToList();

            return groupsByYear.Where(group => groupsUserIsIn.Contains(group.Id)).ToList();
        }

        public List<GroupEntity> GetAllGroups()
        {
            var response = QueryGroupTable();

            return response.Any() ? response.ToList() : new List<GroupEntity>();
        }

        private List<GroupEntity> QueryGroupTable(string filter = "")
        {
            var response = _tableStorageHelper.QueryEntities<GroupEntity>(AppConstants.GroupsTable, string.IsNullOrWhiteSpace(filter) ? null : filter).Result;

            return response.Any() ? response.ToList() : new List<GroupEntity>();
        }

        public string JoinGroup(JoinGroupRequest joinGroupRequest)
        {
            // 1. query group, if it doesn't exist, return empty list

            GroupEntity groupEntity = GetGroup(joinGroupRequest.GroupId);

            if (groupEntity.Id.ToString() == string.Empty) {
                // No group Found
                _logger.LogError(AppConstants.GroupNotFound + " : " + joinGroupRequest.GroupId);
                return AppConstants.GroupNotFound + " : " + joinGroupRequest.GroupId;
            }

            // 2. get all users from group, see if user exists

            List<UserEntity> userEntities = _userService.GetUsers(groupEntity.Id.ToString());

            if (!userEntities.Any()) {
                // no users found in group
                // should be at least 1 (owner)
                _logger.LogError(AppConstants.GroupNoUsersFound + groupEntity.Name);

                return AppConstants.GroupNoUsersFound + groupEntity.Name;
            }

            List<UserEntity> usersGroups = userEntities.Where(user => user.Email == joinGroupRequest.Email && user.Group.ToString() == joinGroupRequest.GroupId).ToList();

            if (usersGroups.Any()) {
                // user already in group
                // do nothing
                return AppConstants.UserAlreadyInGroup + groupEntity.Name;
            }

            // 3. add user to JoinGroupRequest table
            JoinGroupRequestEntity joinGroupRequestEntity = new() {
                PartitionKey = joinGroupRequest.GroupId,
                RowKey = joinGroupRequest.Email,
                GroupId = joinGroupRequest.GroupId,
                Email = joinGroupRequest.Email,
                Timestamp = DateTime.Now,
                ETag = ETag.All
            };

            Response response = _tableStorageHelper.UpsertEntity(joinGroupRequestEntity, AppConstants.JoinGroupRequestsTable).Result;

            return (response != null && !response.IsError) ? AppConstants.Success : AppConstants.JoinGroupError + "email: " + joinGroupRequest.Email + " group: " + joinGroupRequest.GroupId;
        }

        public string LeaveGroup(LeaveGroupRequest leaveGroupRequest)
        {
            // 1. query group, if it doesn't exist, return empty list

            GroupEntity groupEntity = GetGroup(leaveGroupRequest.GroupId);

            if (string.IsNullOrWhiteSpace(groupEntity.Id.ToString())) {
                // No group Found
                _logger.LogError(AppConstants.GroupNotFound + " : " + leaveGroupRequest.GroupId);
                return AppConstants.GroupNotFound + " : " + leaveGroupRequest.GroupId;
            }

            // 2. get all users from group, see if user exists

            List<UserEntity> userEntities = _userService.GetUsers(groupEntity.Id.ToString());

            if (!userEntities.Any()) {
                // no users found in group
                // should be at least 1 (owner)
                _logger.LogError(AppConstants.GroupNoUsersFound + leaveGroupRequest.GroupId);

                return AppConstants.GroupNoUsersFound + leaveGroupRequest.GroupId;
            }

            UserEntity userInGroup = userEntities.FirstOrDefault(user => user.Email == leaveGroupRequest.Email && user.Group.ToString() == leaveGroupRequest.GroupId);

            string filter = TableClient.CreateQueryFilter<DraftEntity>((draft) => draft.Email == leaveGroupRequest.Email && draft.GroupId == Guid.Parse(leaveGroupRequest.GroupId));

            DraftEntity userInDraft = _tableStorageHelper.QueryEntities<DraftEntity>(AppConstants.DraftTable, filter).Result.FirstOrDefault();

            if (userInGroup == null) {
                return AppConstants.SomethingWentWrong;
            }

            // delete user from group 
            if (userInGroup != null) {
                var userDeleteResponse = _tableStorageHelper.DeleteEntity(userInGroup, AppConstants.UsersTable).Result;
                return (userDeleteResponse != null && !userDeleteResponse.IsError) ?
                    AppConstants.Success :
                    AppConstants.LeaveGroupError + "email: " + leaveGroupRequest.Email + " group: " + leaveGroupRequest.GroupId;
            }

            // delete user from group's draft
            if (userInDraft != null) {
                var draftDeleteResponse = _tableStorageHelper.DeleteEntity(userInDraft, AppConstants.DraftTable).Result;
            }

            return AppConstants.LeaveGroupError + "email: " + leaveGroupRequest.Email + " group: " + leaveGroupRequest.GroupId;
        }

        public GroupEntity CreateGroup(string name, string ownerEmail)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(ownerEmail)) {
                return new GroupEntity() {
                    Id = Guid.Empty
                };
            }

            // Query groups first
            // Ensure owner has not created more than MaxGroupsPerOwner groups
            // Create group if validations pass
            // Create user entity for group owner

            string filter = TableClient.CreateQueryFilter<GroupEntity>((group) => group.Owner == ownerEmail);

            var currentGroupsResponse = QueryGroupTable(filter);

            List<GroupEntity> currentGroups = currentGroupsResponse.ToList();

            if (currentGroups.Count > _appConfig.MaxGroupsPerOwner) {
                return new GroupEntity() {
                    Id = Guid.Empty
                };
            }

            Guid guid = Guid.NewGuid();

            GroupEntity groupEntity = new() {
                PartitionKey = guid.ToString(),
                RowKey = Guid.NewGuid().ToString(),
                Id = guid,
                Name = name,
                Owner = ownerEmail,
                Year = AppConstants.CurrentNBASeasonYear,
                DraftDate = AppConstants.NBAStartDate.AddDays(-1), // set default date to day before NBA season starts
                ETag = ETag.All,
                Timestamp = DateTime.Now,
            };

            Response response = _tableStorageHelper.UpsertEntity(groupEntity, AppConstants.GroupsTable).Result;

            if (response == null || response.IsError) {
                return new GroupEntity() {
                    Id = Guid.Empty
                };
            }

            User owner = new() {
                Email = ownerEmail,
                Team = string.Empty,
                Group = groupEntity.Id.ToString()
            };

            User userResult = _userService.UpsertUser(owner);

            if (userResult.Email != owner.Email) {
                _logger.LogError(AppConstants.SomethingWentWrong);
            }

            return groupEntity;
        }

        public GroupEntity UpsertGroup(GroupEntity group)
        {
            if (group.Id != Guid.Empty) {
                GroupEntity groupEntity = new() {
                    PartitionKey = group.PartitionKey,
                    RowKey = group.RowKey,
                    Id = group.Id,
                    Name = group.Name,
                    Owner = group.Owner,
                    Year = group.Year,
                    DraftDate = group.DraftDate,
                    ETag = ETag.All,
                    Timestamp = DateTime.Now,
                };

                Response response = _tableStorageHelper.UpsertEntity(groupEntity, AppConstants.GroupsTable).Result;

                return (response != null && !response.IsError) ? group : new GroupEntity();
            }

            return new GroupEntity();
        }

        public string ApproveNewGroupMember(ApproveUserRequest approveUserRequest)
        {
            // admins needs away to approve people who clicked the group invitation link / request to join group
            // validation invite ID is correct ???

            // does request exist?
            List<JoinGroupRequestEntity> joinGroupRequests = GetJoinGroupRequests(approveUserRequest.GroupId);

            if (!joinGroupRequests.Select(x => x.GroupId == approveUserRequest.GroupId && x.Email == approveUserRequest.Email).Any()) {
                // No request found
                _logger.LogError(AppConstants.GroupNotFound + " : " + approveUserRequest.GroupId + " during ApproveNewGroupMember");
                return AppConstants.GroupNotFound + " : " + approveUserRequest.GroupId;
            }

            // validate group exists
            GroupEntity groupEntity = GetGroup(approveUserRequest.GroupId);

            if (groupEntity.Id.ToString() == string.Empty) {
                // No group Found
                _logger.LogError(AppConstants.GroupNotFound + " : " + approveUserRequest.GroupId + " during ApproveNewGroupMember");
                return AppConstants.GroupNotFound + " : " + approveUserRequest.GroupId;
            }

            // validate admin is owner
            if (groupEntity.Owner != approveUserRequest.AdminEmail) {
                // owner not approving
                _logger.LogError(AppConstants.NotOwner + "user: " + approveUserRequest.AdminEmail + " : " + approveUserRequest.GroupId + " during ApproveNewGroupMember");
                return AppConstants.NotOwner + "user: " + approveUserRequest.AdminEmail + " : " + approveUserRequest.GroupId;
            }

            // validate user does not already belong to group

            List<UserEntity> userEntities = _userService.GetUsers(groupEntity.Id.ToString());

            if (!userEntities.Any()) {
                // no users found in group
                // should be at least 1 (owner)
                _logger.LogError(AppConstants.GroupNoUsersFound + groupEntity.Name);

                return AppConstants.GroupNoUsersFound + groupEntity.Name;
            }

            List<UserEntity> usersGroups = userEntities.Where(user => user.Email == approveUserRequest.Email && user.Group.ToString() == approveUserRequest.GroupId).ToList();

            if (usersGroups.Any()) {
                // user already in group
                // do nothing
                return AppConstants.UserAlreadyInGroup + groupEntity.Name;
            }

            // Add user and group data to user table
            UserEntity userEntity = new() {
                PartitionKey = approveUserRequest.GroupId,
                RowKey = approveUserRequest.Email,
                Email = approveUserRequest.Email,
                Group = Guid.Parse(approveUserRequest.GroupId),
                ETag = ETag.All,
                Timestamp = DateTime.Now
            };

            Response upsertUserResponse = _tableStorageHelper.UpsertEntity(userEntity, AppConstants.UsersTable).Result;

            if (upsertUserResponse == null || upsertUserResponse.IsError) {
                return AppConstants.UsersCouldNotBeUpdated + "email: " + approveUserRequest.Email + " group: " + approveUserRequest.GroupId;
            }

            // remove join group request from storage
            JoinGroupRequestEntity joinGroupRequestEntity = new() {
                PartitionKey = approveUserRequest.GroupId,
                RowKey = approveUserRequest.Email,
                GroupId = approveUserRequest.GroupId,
                Email = approveUserRequest.Email,
                Timestamp = DateTime.Now,
                ETag = ETag.All
            };

            Response deleteResponse = _tableStorageHelper.DeleteEntity(joinGroupRequestEntity, AppConstants.JoinGroupRequestsTable).Result;

            if (deleteResponse == null || deleteResponse.IsError) {
                return AppConstants.JoinGroupError + "email: " + approveUserRequest.Email + " group: " + approveUserRequest.GroupId;
            }

            return AppConstants.Success;
        }

        public List<JoinGroupRequestEntity> GetJoinGroupRequests(string groupId, string filter = "")
        {
            string groupFilter = TableClient.CreateQueryFilter<JoinGroupRequestEntity>((group) => group.GroupId == groupId);

            var response = _tableStorageHelper.QueryEntities<JoinGroupRequestEntity>(AppConstants.JoinGroupRequestsTable, groupFilter).Result;

            return response.Any() ? response.ToList() : new List<JoinGroupRequestEntity>();
        }

        private static int PreseasonValue(int value)
        {
            return DateTime.Now < AppConstants.NBAStartDate ? 0 : value;
        }

        private static string PreseasonPlayoffs(string value)
        {
            return DateTime.Now < AppConstants.NBAStartDate ? "" : value;
        }
    }
}
