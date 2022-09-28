using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface ILeagueDataAccess
    {
        List<LeagueStandings> GetLeagueStandings();
        User DraftTeam(User user);
        List<Draft> SetupDraft();
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

        public User DraftTeam(User user)
        {
            if (ValidateUserCanDraftTeam(user).Count == 0) {
                return user;
            } else {
                return new User();
            }
        }

        public List<Draft> SetupDraft()
        {
            return new List<Draft>();
        }

        private List<string> ValidateUserCanDraftTeam(User user)
        {
            List<string> validations = new();

            // Validations:
            // 1. Is it the user's turn to draft? 
            //      1.1 Time Now is greater than DraftStartHour and less than DraftStartHour + DraftWindowMinutes
            // 2. Has the user already drafted?
            //      2.1 User email is not already present in Governer column (maybe there is another way to do this?)
            // 3. Is the team available?
            //      3.1 Governer column is BLANK next to team. Could be good to do it this way, since it is one query for #2 and #3

            return validations;
        }

        private static int PreseasonValue(int value)
        {
            DateTime nbaStartDate = new(2022, 10, 18); // nba start date

            if (DateTime.Now < nbaStartDate) {
                return 0;
            } else {
                return value;
            }
        }

        private static string PreseasonPlayoffs(string value)
        {
            DateTime nbaStartDate = new(2022, 10, 18);

            if (DateTime.Now < nbaStartDate) {
                return "";
            } else {
                return value;
            }
        }
    }
}
