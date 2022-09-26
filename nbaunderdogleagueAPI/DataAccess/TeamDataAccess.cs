using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface ITeamDataAccess
    {
        List<Standings> GetStandingsData();
        List<TeamEntity> GetTeams();
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
    }
    public class TeamDataAccess : ITeamDataAccess
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ITableStorageHelper _tableStorageHelper;
        public TeamDataAccess(IOptions<AppConfig> options, ILogger<TeamDataAccess> logger, IUserService userService, ITableStorageHelper tableStorageHelper) 
        {
            _appConfig = options.Value;
            _logger = logger;
            _userService = userService;
            _tableStorageHelper = tableStorageHelper;
        }
        public List<Standings> GetStandingsData()
        {
            List<Standings> standings = new List<Standings>();

            List<TeamEntity> teamsEntities = GetTeams();

            List<UserEntity> users = _userService.GetUsers();

            return standings;

        }

        public List<TeamEntity> GetTeams()
        {
            var response = _tableStorageHelper.QueryEntities<TeamEntity>(AppConstants.TeamsTable).Result;

            return response.ToList();
        }

        public List<TeamEntity> AddTeams(List<TeamEntity> teamEntities)
        {
            var response = _tableStorageHelper.UpsertEntities(teamEntities, AppConstants.TeamsTable).Result;

            return (response != null && !response.GetRawResponse().IsError) ? teamEntities : new List<TeamEntity>();
        }
    }
}
