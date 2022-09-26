using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Models.NBAModels;
using nbaunderdogleagueAPI.Services;
using System.Collections.Generic;
using System.Text.Json;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface ITeamDataAccess
    {
        List<Standings> GetStandingsData();
        Task<List<CurrentNBAStandings>> GetCurrentNBAStandings();
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
            List<Standings> standings = new();
            List<CurrentNBAStandings> currentNBAStandings = GetCurrentNBAStandings().Result;
            /*
             *         
                public string Governor { get; set; }
                public string TeamName { get; set; }
                public string TeamCity { get; set; }
                public int ProjectedWin { get; set; }
                public int ProjectedLoss { get; set; }
                public int Win { get; set; }
                public int Loss { get; set; }
                public string Playoffs { get; set; }
             * 
             */
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

        public async Task<List<CurrentNBAStandings>> GetCurrentNBAStandings()
        {
            HttpClient httpClient = new();

            Stream stream = await httpClient.GetStreamAsync(AppConstants.CurrentNBAStandingsJSON);

            using JsonDocument resp = await JsonDocument.ParseAsync(stream);

            Root data = resp.Deserialize<Root>();

            List<CurrentNBAStandings> currentNBAStandings = new();

            foreach (Team team in data.league.standard.teams) 
            {
                currentNBAStandings.Add(new() {
                    TeamName = team.teamSitesOnly.teamNickname,
                    TeamCity = team.teamSitesOnly.teamName,
                    Win = int.Parse(team.win),
                    Loss = int.Parse(team.loss),
                    Playoffs = team.clinchedPlayoffsCode
                });
            }

            return currentNBAStandings;
        }
    }
}
