using Azure;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using Newtonsoft.Json;
using static nbaunderdogleagueAPI.Models.RapidAPI_NBA.RapidAPI_NBA;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface INBADataAccess
    {
        GameResponse GetGamesFromRapidAPI();
        List<NBAGameEntity> UpdateGamesFromRapidAPI();
        Task<RapidAPIContent> GetNBAGamesDataFromRapidAPI(DateTime date);
        List<TeamStats> UpdateTeamStatsFromRapidAPI();
        List<Scoreboard> NBAScoreboard(string groupId);
    }
    public class NBADataAccess : INBADataAccess
    {
        private readonly ILogger _logger;
        private readonly ITableStorageHelper _tableStorageHelper;
        private readonly IRapidAPIHelper _rapidAPIHelper;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly AppConfig _appConfig;
        public NBADataAccess(IOptions<AppConfig> appConfig, ILogger<NBADataAccess> logger, ITableStorageHelper tableStorageHelper, IUserService userService, IRapidAPIHelper rapidAPIHelper, ITeamService teamService)
        {
            _logger = logger;
            _tableStorageHelper = tableStorageHelper;
            _appConfig = appConfig.Value;
            _userService = userService;
            _rapidAPIHelper = rapidAPIHelper;
            _teamService = teamService;
        }

        public GameResponse GetGamesFromRapidAPI()
        {
            try {
                DateTime now = DateTime.UtcNow;
                DateTime dayBefore = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-1); // get yesterday's games

                Game.Root output;

                RapidAPIContent content = GetNBAGamesDataFromRapidAPI(dayBefore).Result;

                if (string.IsNullOrEmpty(content.Content)) {
                    return new GameResponse();
                }

                output = JsonConvert.DeserializeObject<Game.Root>(content.Content);

                List<Game.Response> games = output.response;

                return new GameResponse() {
                    Games = games,
                    RequestsRemaining = content.RequestsRemaining
                };
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new GameResponse();
        }

        public List<TeamStats> UpdateTeamStatsFromRapidAPI()
        {
            if (!_rapidAPIHelper.IsRapidAPIAvailable()) {
                return new List<TeamStats>();
            }

            TeamStatsResponse teamStatsResponse = GetTeamStatsFromRapidAPI();
            List<TeamStats> teamStats = teamStatsResponse.TeamStats.OrderByDescending(team => team.Wins).ToList();

            if (teamStats.Count != 0) {
                List<ManualTeamStatsEntity> manualTeamStats = new();

                if (teamStats.Count == 30) {
                    teamStats.ForEach(teamData => manualTeamStats.Add(new ManualTeamStatsEntity() {
                        PartitionKey = "TeamStats",
                        RowKey = teamData.TeamName,
                        TeamID = teamData.TeamID,
                        TeamCity = teamData.TeamCity,
                        TeamName = teamData.TeamName,
                        Conference = teamData.Conference,
                        Wins = teamData.Wins,
                        //PlayoffWins = teamData.PlayoffWins, // need to update manually, missing from API endpoint
                        Losses = teamData.Losses,
                        Standing = teamData.Standing,
                        Ratio = teamData.Ratio,
                        Streak = teamData.Streak,
                        //ClinchedPlayoffBirth = teamData.ClinchedPlayoffBirth, // need to update manually, missing from API endpoint
                        Logo = teamData.Logo,
                        ETag = ETag.All,
                        Timestamp = DateTime.Now
                    }));

                    var updateTeamStatsManuallyResponse = _tableStorageHelper.UpsertEntities(manualTeamStats, AppConstants.ManualTeamStats).Result;

                    return (updateTeamStatsManuallyResponse == AppConstants.Success) ? teamStats : new List<TeamStats>();
                } else {
                    _logger.LogError("Team Stats not fetched for all teams, count: " + teamStats.Count);
                }
            } else {
                // Start of new season
                // reset team data:

                // only update August onward
                if (AppConstants.CurrentDate.Month >= 8) {
                    List<TeamEntity> currentTeamStats = _teamService.GetTeams();

                    List<ManualTeamStatsEntity> manualTeamStats = new();

                    if (currentTeamStats.Count == 30) {
                        currentTeamStats.ForEach(teamData => manualTeamStats.Add(new ManualTeamStatsEntity() {
                            PartitionKey = "TeamStats",
                            RowKey = teamData.Name,
                            Wins = 0,
                            PlayoffWins = 0,
                            Losses = 0,
                            Standing = 0,
                            Ratio = 0,
                            Streak = 0,
                            ClinchedPlayoffBirth = 0,
                            ETag = ETag.All,
                            Timestamp = DateTime.Now
                        }));

                        var updateTeamStatsManuallyResponse = _tableStorageHelper.UpsertEntities(manualTeamStats, AppConstants.ManualTeamStats).Result;

                        return (updateTeamStatsManuallyResponse == AppConstants.Success) ? teamStats : new List<TeamStats>();
                    }
                }
            }

            return new List<TeamStats>();
        }

        // Only save games from the previous day
        // if there are no games that do, do not overwrite 
        // this data is just for the scoreboard on the UI
        public List<NBAGameEntity> UpdateGamesFromRapidAPI()
        {
            // Rapid API request limit has been met
            // do not update
            if (!_rapidAPIHelper.IsRapidAPIAvailable()) {
                return new List<NBAGameEntity>();
            }

            try {
                GameResponse gameResponse = GetGamesFromRapidAPI();
                List<Game.Response> games = gameResponse.Games;

                // replace current games in scoreboard if there are new games
                //  otherwise, keep most recent games
                // keep 10 most recent games for scoreboard
                if (games.Count > 0) {
                    List<NBAGameEntity> currentScoreboard = _tableStorageHelper.QueryEntities<NBAGameEntity>(AppConstants.ScoreboardTable)
                                                            .Result
                                                            .OrderBy(x => x.Timestamp) // oldest dates first
                                                            .ToList();

                    _tableStorageHelper.DeleteAllEntities(currentScoreboard.Take(games.Count).ToList(), AppConstants.ScoreboardTable);

                    List<NBAGameEntity> nbaGameEntities = new();

                    games.ForEach(g => nbaGameEntities.Add(new NBAGameEntity() {
                        PartitionKey = "NBA",
                        RowKey = g.id.ToString(),
                        HomeTeam = g.teams.home.nickname,
                        HomeLogo = g.teams.home.logo,
                        HomeScore = g.scores.home.points,
                        VisitorsTeam = g.teams.visitors.nickname,
                        VisitorsLogo = g.teams.visitors.logo,
                        VisitorsScore = g.scores.visitors.points,
                        ETag = ETag.All,
                        Timestamp = DateTime.Now
                    }));

                    var updateGamesResponse = _tableStorageHelper.UpsertEntities(nbaGameEntities, AppConstants.ScoreboardTable).Result;

                    return (updateGamesResponse == AppConstants.Success) ? nbaGameEntities : new List<NBAGameEntity>();
                } else {
                    _logger.LogInformation("No new games on: " + DateTime.Now.ToString());
                }
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new List<NBAGameEntity>();
        }

        private TeamStatsResponse GetTeamStatsFromRapidAPI()
        {
            try {
                // season starts in October, switch season on site in September
                DateTimeOffset now = DateTimeOffset.UtcNow;
                string season = now.Month >= 9 ? now.Year.ToString() : (now.Year - 1).ToString();

                string apiURL = "https://api-nba-v1.p.rapidapi.com/standings";
                string parameterString = "?league=standard&season=" + season;

                RapidAPIContent content = _rapidAPIHelper.QueryRapidAPI(apiURL, parameterString).Result;

                if (string.IsNullOrEmpty(content.Content)) {
                    return new TeamStatsResponse();
                }

                Standings.Root output = JsonConvert.DeserializeObject<Standings.Root>(content.Content);

                List<TeamStats> teamStats = output.ExtractTeamStats(_logger);

                return new TeamStatsResponse() {
                    TeamStats = teamStats,
                    RequestsRemaining = content.RequestsRemaining
                };
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new TeamStatsResponse();
        }

        public async Task<RapidAPIContent> GetNBAGamesDataFromRapidAPI(DateTime date)
        {
            try {
                string dateString = date.ToString("yyyy-MM-dd");

                string apiURL = "https://api-nba-v1.p.rapidapi.com/games";
                string parameterString = "?date=" + dateString;

                return await _rapidAPIHelper.QueryRapidAPI(apiURL, parameterString);
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public List<Scoreboard> NBAScoreboard(string groupId = null)
        {
            try {
                List<NBAGameEntity> nbaGameEntities = _tableStorageHelper.QueryEntities<NBAGameEntity>(AppConstants.ScoreboardTable)
                                            .Result
                                            .OrderBy(x => x.Timestamp) // oldest dates first
                                            .ToList();

                List<Scoreboard> scoreboard = new();
                Dictionary<string, UserEntity> teamUserDict = new();

                if (!string.IsNullOrEmpty(groupId)) {
                    List<UserEntity> users = _userService.GetUsers(groupId);

                    users.ForEach(user => {
                        teamUserDict.Add(user.Team, user);
                    });
                }

                foreach (NBAGameEntity nba in nbaGameEntities) {
                    scoreboard.Add(new Scoreboard() {
                        HomeGovernor = teamUserDict.ContainsKey(nba.HomeTeam)
                                        ? teamUserDict[nba.HomeTeam].Username ?? teamUserDict[nba.HomeTeam].Email?.Split('@')[0]
                                        : nba.HomeTeam,
                        HomeLogo = nba.HomeLogo,
                        HomeTeam = nba.HomeTeam,
                        HomeScore = nba.HomeScore,
                        VisitorsGovernor = teamUserDict.ContainsKey(nba.VisitorsTeam)
                                        ? teamUserDict[nba.VisitorsTeam].Username ?? teamUserDict[nba.VisitorsTeam].Email?.Split('@')[0]
                                        : nba.VisitorsTeam,
                        VisitorsLogo = nba.VisitorsLogo,
                        VisitorsTeam = nba.VisitorsTeam,
                        VisitorsScore = nba.VisitorsScore,
                        GameDate = (DateTimeOffset)nba.Timestamp,
                    });
                }

                return scoreboard.OrderByDescending(x => x.GameDate).ToList();
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new List<Scoreboard>();
        }
    }
}
