using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface ILeagueDataAccess
    {
        List<LeagueStandings> GetLeagueStandings(string leagueId);
        LeagueInfo CreateLeague(string name, string ownerEmail);
        LeagueEntity GetLeague(string leagueId);
        List<LeagueEntity> GetAllLeaguesByYear(int year);
        List<LeagueEntity> GetAllLeagues();
        string JoinLeague(string id, string email);
    }
    public class LeagueDataAccess : ILeagueDataAccess
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly ITableStorageHelper _tableStorageHelper;
        public LeagueDataAccess(IOptions<AppConfig> options, ILogger<TeamDataAccess> logger, IUserService userService, ITeamService teamService, ITableStorageHelper tableStorageHelper)
        {
            _appConfig = options.Value;
            _logger = logger;
            _userService = userService;
            _teamService = teamService;
            _tableStorageHelper = tableStorageHelper;
        }

        public List<LeagueStandings> GetLeagueStandings(string leagueId)
        {
            List<LeagueStandings> standings = new();

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

                standings.Add(new LeagueStandings() {
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

            return standings.OrderByDescending(league => league.Score).ToList();
        }

        public LeagueEntity GetLeague(string leagueId)
        {
            string filter = TableClient.CreateQueryFilter<LeagueEntity>((league) => league.PartitionKey == leagueId);

            var response = _tableStorageHelper.QueryEntities<LeagueEntity>(AppConstants.LeaguesTable, filter).Result;

            return response.Any() ? response.ToList()[0] : new LeagueEntity();
        }

        public List<LeagueEntity> GetAllLeaguesByYear(int year)
        {
            string filter = TableClient.CreateQueryFilter<LeagueEntity>((league) => league.Year == year);

            var response = _tableStorageHelper.QueryEntities<LeagueEntity>(AppConstants.LeaguesTable, filter).Result;

            return response.Any() ? response.ToList() : new List<LeagueEntity>();
        }

        public List<LeagueEntity> GetAllLeagues()
        {
            var response = _tableStorageHelper.QueryEntities<LeagueEntity>(AppConstants.LeaguesTable).Result;

            return response.Any() ? response.ToList() : new List<LeagueEntity>();
        }

        public string JoinLeague(string leagueId, string email)
        {
            // 1. query league, if it doesn't exist, return empty list

            LeagueEntity leagueEntity = GetLeague(leagueId);

            if (leagueEntity.Id.ToString() == string.Empty) {
                // No League Found
                _logger.LogError(AppConstants.LeagueNotFound + " : " + leagueId);
                return AppConstants.LeagueNotFound + " : " + leagueId;
            }

            // 2. get all users from league, see if user doesn't already exist

            List<UserEntity> userEntities = _userService.GetUsers(leagueEntity.Id.ToString());

            if (!userEntities.Any()) {
                // no users found in league
                // should be at least 1 (owner)
                _logger.LogError(AppConstants.LeagueNoUsersFound + leagueId);

                return AppConstants.LeagueNoUsersFound + leagueId;
            }

            if (userEntities.Select(user => user.Email == email).Any()) {
                // user already in league
                // do nothing
                return AppConstants.UserAlreadyInLeague;
            }

            // 3. add league to user data
            UserEntity userEntity = new() {
                PartitionKey = leagueId,
                RowKey = email,
                Email = email,
                League = Guid.Parse(leagueId),
                ETag = ETag.All,
                Timestamp = DateTime.Now
            };

            var response = _tableStorageHelper.UpsertEntity(userEntity, AppConstants.UsersTable).Result;

            return (response != null && !response.IsError) ? AppConstants.Success : AppConstants.JoinLeagueError + "email: " + email + " league: " + leagueId;
        }

        public LeagueInfo CreateLeague(string name, string ownerEmail)
        {
            LeagueInfo league = new() {
                Id = Guid.NewGuid(),
                Year = DateTime.Now.Year,
                Name = name,
                Owner = ownerEmail
            };

            LeagueEntity leagueEntity = new() {
                PartitionKey = league.Id.ToString(),
                RowKey = Guid.NewGuid().ToString(),
                Id = league.Id,
                Name = league.Name,
                Owner = league.Owner,
                Year = league.Year,
                ETag = ETag.All,
                Timestamp = DateTime.Now,
            };

            Response response = _tableStorageHelper.UpsertEntity(leagueEntity, AppConstants.LeaguesTable).Result;

            return (response != null && !response.IsError) ? league : new LeagueInfo();
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
