using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface ITeamDataAccess
    {
        List<Team> GetTeamData();
        Task<List<TeamsEntity>> AddTeamsAsync(List<TeamsEntity> teamsEntities);
    }
    public class TeamDataAccess : ITeamDataAccess
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        public TeamDataAccess(IOptions<AppConfig> options, ILogger<TeamDataAccess> logger) 
        {
            _appConfig = options.Value;
            _logger = logger;
        }
        public List<Team> GetTeamData()
        {
            List<Team> teamData = new()
            {
                new Team()
                {
                    TeamId = 001,
                    TeamName = "Lakers",
                    TeamCity = "Los Angeles",
                    ProjectedWin = 41,
                    ProjectedLoss = 41,
                    Win = 20,
                    Loss = 10,
                    Playoffs = "C"
                    
                },
                new Team()
                {
                    TeamId = 002,
                    TeamName = "Suns",
                    TeamCity = "Pheonix",
                    ProjectedWin = 25,
                    ProjectedLoss = 30,
                    Win = 15,
                    Loss = 15,
                    Playoffs = ""
                }
            };

            return teamData;
        }

        public async Task<List<TeamsEntity>> AddTeamsAsync(List<TeamsEntity> teamsEntities)
        {
            try {
                TableClient tableClient = new TableClient(_appConfig.TableConnection, AppConstants.TeamsTable);
                await tableClient.CreateIfNotExistsAsync();

                foreach (TeamsEntity team in teamsEntities) {

                    var response = tableClient.AddEntityAsync(team).Result;

                    if (response.IsError) {
                        _logger.LogError(response.ReasonPhrase);
                    }
                }

                return teamsEntities;

            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new List<TeamsEntity>();
        }
    }
}
