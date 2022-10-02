using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using System.Text.RegularExpressions;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IGroupDataAccess
    {
        List<GroupStandings> GetGroupStandings(string groupId);
        GroupEntity CreateGroup(string name, string ownerEmail);
        GroupEntity GetGroup(string groupId);
        List<GroupEntity> GetAllGroupsByYear(int year);
        List<GroupEntity> GetAllGroups();
        string JoinGroup(string id, string email);
    }
    public class GroupDataAccess : IGroupDataAccess
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly ITableStorageHelper _tableStorageHelper;
        public GroupDataAccess(IOptions<AppConfig> options, ILogger<TeamDataAccess> logger, IUserService userService, ITeamService teamService, ITableStorageHelper tableStorageHelper)
        {
            _appConfig = options.Value;
            _logger = logger;
            _userService = userService;
            _teamService = teamService;
            _tableStorageHelper = tableStorageHelper;
        }

        public List<GroupStandings> GetGroupStandings(string groupId)
        {
            List<GroupStandings> standings = new();

            // 1. Get Current NBA Standings Data (from NBA stats)
            Dictionary<string, CurrentNBAStanding> currentNBAStandingsDict = _teamService.GetCurrentNBAStandingsDictionary();

            // 2. Get Projected Data (from storage)
            List<TeamEntity> teamsEntities = _teamService.GetTeamsEntity();

            // 3. Combine 1 and 2
            foreach (TeamEntity team in teamsEntities) {
                CurrentNBAStanding currentNBAStanding = currentNBAStandingsDict[team.Name];

                int win = currentNBAStanding.Win; //PreseasonValue(currentNBAStanding.Win);
                int loss = currentNBAStanding.Loss; //PreseasonValue(currentNBAStanding.Loss);
                int projectedWin = team.ProjectedWin;
                int projectedLoss = team.ProjectedLoss;

                double projectedDiff = (double)(projectedWin / (double)(projectedWin + projectedLoss));
                double actualDiff = (double)(win / (double)(win + loss));
                double score = (double)(actualDiff / (double)projectedDiff);

                string playoffs = PreseasonPlayoffs(currentNBAStanding.Playoffs);

                standings.Add(new GroupStandings() {
                    Governor = "", // Could add user here or in UI
                    TeamName = team.Name,
                    TeamCity = team.City,
                    ProjectedWin = projectedWin,
                    ProjectedLoss = projectedLoss,
                    Win = win,
                    Loss = loss,
                    Score = Math.Round(score, 2),
                    Playoffs = playoffs
                });
            }

            return standings.OrderByDescending(group => group.Score).ToList();
        }

        public GroupEntity GetGroup(string groupId)
        {
            string filter = TableClient.CreateQueryFilter<GroupEntity>((group) => group.PartitionKey == groupId);

            var response = _tableStorageHelper.QueryEntities<GroupEntity>(AppConstants.GroupsTable, filter).Result;

            return response.Any() ? response.ToList()[0] : new GroupEntity();
        }

        public List<GroupEntity> GetAllGroupsByYear(int year)
        {
            string filter = TableClient.CreateQueryFilter<GroupEntity>((group) => group.Year == year);

            var response = _tableStorageHelper.QueryEntities<GroupEntity>(AppConstants.GroupsTable, filter).Result;

            return response.Any() ? response.ToList() : new List<GroupEntity>();
        }

        public List<GroupEntity> GetAllGroups()
        {
            var response = _tableStorageHelper.QueryEntities<GroupEntity>(AppConstants.GroupsTable).Result;

            return response.Any() ? response.ToList() : new List<GroupEntity>();
        }

        public string JoinGroup(string groupId, string email)
        {
            // 1. query group, if it doesn't exist, return empty list

            GroupEntity groupEntity = GetGroup(groupId);

            if (groupEntity.Id.ToString() == string.Empty) {
                // No group Found
                _logger.LogError(AppConstants.GroupNotFound + " : " + groupId);
                return AppConstants.GroupNotFound + " : " + groupId;
            }

            // 2. get all users from group, see if user doesn't already exist

            List<UserEntity> userEntities = _userService.GetUsers(groupEntity.Id.ToString());

            if (!userEntities.Any()) {
                // no users found in group
                // should be at least 1 (owner)
                _logger.LogError(AppConstants.GroupNoUsersFound + groupId);

                return AppConstants.GroupNoUsersFound + groupId;
            }

            if (userEntities.Select(user => user.Email == email).Any()) {
                // user already in group
                // do nothing
                return AppConstants.UserAlreadyInGroup;
            }

            // 3. add group to user data
            UserEntity userEntity = new() {
                PartitionKey = groupId,
                RowKey = email,
                Email = email,
                Group = Guid.Parse(groupId),
                ETag = ETag.All,
                Timestamp = DateTime.Now
            };

            var response = _tableStorageHelper.UpsertEntity(userEntity, AppConstants.UsersTable).Result;

            return (response != null && !response.IsError) ? AppConstants.Success : AppConstants.JoinGroupError + "email: " + email + " group: " + groupId;
        }

        public GroupEntity CreateGroup(string name, string ownerEmail)
        {
            // Query groups first
            // Ensure owner has not created more than MaxGroupsPerOwner groups
            // Create group if validations pass

            string filter = TableClient.CreateQueryFilter<GroupEntity>((group) => group.Owner == ownerEmail);

            var currentGroupsResponse = _tableStorageHelper.QueryEntities<GroupEntity>(AppConstants.GroupsTable, filter).Result;

            List<GroupEntity> currentGroups = currentGroupsResponse.ToList();

            if (currentGroups.Count > _appConfig.MaxGroupsPerOwner) {
                return new GroupEntity();
            }

            Guid guid = Guid.NewGuid();

            GroupEntity groupEntity = new() {
                PartitionKey = guid.ToString(),
                RowKey = Guid.NewGuid().ToString(),
                Id = guid,
                Name = name,
                Owner = ownerEmail,
                Year = DateTime.Now.Year,
                ETag = ETag.All,
                Timestamp = DateTime.Now,
            };

            Response response = _tableStorageHelper.UpsertEntity(groupEntity, AppConstants.GroupsTable).Result;

            return (response != null && !response.IsError) ? groupEntity : new GroupEntity();
        }

        private static int PreseasonValue(int value)
        {
            DateTime nbaStartDate = new(DateTime.Now.Year, 10, 18); // nba start date

            if (DateTime.Now < nbaStartDate) {
                return 0;
            } else {
                return value;
            }
        }

        private static string PreseasonPlayoffs(string value)
        {
            DateTime nbaStartDate = new(DateTime.Now.Year, 10, 18);

            if (DateTime.Now < nbaStartDate) {
                return "";
            } else {
                return value;
            }
        }
    }
}
