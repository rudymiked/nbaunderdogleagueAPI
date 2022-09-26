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
        Task<Dictionary<string, CurrentNBAStanding>> GetCurrentNBAStandings();
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

            // Get Current NBA Standings Data (from NBA stats)
            Dictionary<string, CurrentNBAStanding> currentNBAStandingsDict = GetCurrentNBAStandings().Result;

            // Get Projected Data (from storage)
            List<TeamEntity> teamsEntities = GetTeams();

            // Combine
            foreach(TeamEntity team in teamsEntities) {
                CurrentNBAStanding currentNBAStanding = currentNBAStandingsDict[team.Name];

                standings.Add(new Standings() {
                    Governor = "", // Could add user here or in UI
                    TeamName = team.Name,
                    TeamCity = team.City,
                    ProjectedWin = team.ProjectedWin,
                    ProjectedLoss = team.ProjectedLoss,
                    Win = currentNBAStanding.Win,
                    Loss = currentNBAStanding.Loss,
                    Playoffs = currentNBAStanding.Playoffs
                });
            }

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

        public async Task<Dictionary<string, CurrentNBAStanding>> GetCurrentNBAStandings()
        {
            HttpClient httpClient = new();

            Stream stream = await httpClient.GetStreamAsync(AppConstants.CurrentNBAStandingsJSON);

            using JsonDocument resp = await JsonDocument.ParseAsync(stream);

            Root data = resp.Deserialize<Root>();

            Dictionary<string, CurrentNBAStanding> currentNBAStandingsDict = new();

            foreach (Team team in data.league.standard.teams) {
                currentNBAStandingsDict.Add(team.teamSitesOnly.teamNickname, new() {
                    TeamName = team.teamSitesOnly.teamNickname,
                    TeamCity = team.teamSitesOnly.teamName,
                    Win = int.Parse(team.win),
                    Loss = int.Parse(team.loss),
                    Playoffs = team.clinchedPlayoffsCode
                });
            }

            return currentNBAStandingsDict;
        }
    }
}
