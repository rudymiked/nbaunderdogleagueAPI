using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface ILeagueDataAccess
    {
        List<LeagueStandings> GetLeagueStandings();
    }
    public class LeagueDataAccess : ILeagueDataAccess
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        public LeagueDataAccess(IOptions<AppConfig> options, ILogger<TeamDataAccess> logger, IUserService userService, ITeamService teamService)
        {
            _appConfig = options.Value;
            _logger = logger;
            _userService = userService;
            _teamService = teamService;
        }

        public List<LeagueStandings> GetLeagueStandings()
        {
            List<LeagueStandings> standings = new();

            // Get Current NBA Standings Data (from NBA stats)
            Dictionary<string, CurrentNBAStanding> currentNBAStandingsDict = _teamService.GetCurrentNBAStandingsDictionary();

            // Get Projected Data (from storage)
            List<TeamEntity> teamsEntities = _teamService.GetTeamsEntity();

            // Combine
            foreach (TeamEntity team in teamsEntities) {
                CurrentNBAStanding currentNBAStanding = currentNBAStandingsDict[team.Name];

                standings.Add(new LeagueStandings() {
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
    }
}
