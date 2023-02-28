using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using Newtonsoft.Json;
using static nbaunderdogleagueAPI.Models.RapidAPI_NBA;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface INBADataAccess
    {
        GameResponse GetGamesFromRapidAPI();
        List<NBAGameEntity> UpdateGamesFromRapidAPI();
        Task<RapidAPIContent> GetNBAGamesDataFromRapidAPI(DateTime date);
        List<TeamStats> UpdateTeamStatsFromRapidAPI();
        Task<RapidAPIContent> GetNBAStandingsDataFromRapidAPI(string season);
        List<Scoreboard> NBAScoreboard(string groupId);
        bool SetRapidAPITimeout(DateTimeOffset timeout);
        bool IsRapidAPIAvailable();

    }
    public class NBADataAccess : INBADataAccess
    {
        private readonly ILogger _logger;
        private readonly ITableStorageHelper _tableStorageHelper;
        private readonly IUserService _userService;
        private readonly AppConfig _appConfig;
        public NBADataAccess(IOptions<AppConfig> appConfig, ILogger<NBADataAccess> logger, ITableStorageHelper tableStorageHelper, IUserService userService)
        {
            _logger = logger;
            _tableStorageHelper = tableStorageHelper;
            _appConfig = appConfig.Value;
            _userService = userService;
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
            if (!IsRapidAPIAvailable()) {
                return new List<TeamStats>();
            }

            TeamStatsResponse teamStatsResponse = GetTeamStatsFromRapidAPI();
            List<TeamStats> teamStats = teamStatsResponse.TeamStats.OrderByDescending(team => team.Wins).ToList();

            List<ManualTeamStatsEntity> manualTeamStats = new();

            teamStats.ForEach(teamData => manualTeamStats.Add(new ManualTeamStatsEntity() {
                PartitionKey = "TeamStats",
                RowKey = teamData.TeamName,
                TeamID = teamData.TeamID,
                TeamCity = teamData.TeamCity,
                TeamName = teamData.TeamName,
                Conference = teamData.Conference,
                Wins = teamData.Wins,
                PlayoffWins = teamData.PlayoffWins,
                Losses = teamData.Losses,
                Standing = teamData.Standing,
                Ratio = teamData.Ratio,
                Streak = teamData.Streak,
                ClinchedPlayoffBirth = teamData.ClinchedPlayoffBirth,
                Logo = teamData.Logo,
                ETag = ETag.All,
                Timestamp = DateTime.Now
            }));

            if (teamStats.Count != 0) {
                if (teamStats.Count == 30) {
                    var updateTeamStatsManuallyResponse = _tableStorageHelper.UpsertEntities(manualTeamStats, AppConstants.ManualTeamStats).Result;

                    return (updateTeamStatsManuallyResponse != null && !updateTeamStatsManuallyResponse.GetRawResponse().IsError) ? teamStats : new List<TeamStats>();
                } else {
                    _logger.LogError("Team Stats not fetched for all teams, count: " + teamStats.Count);
                }
            } else {
                _logger.LogError("Team Stats is zero");
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
            if (!IsRapidAPIAvailable()) {
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

                    return (updateGamesResponse != null && !updateGamesResponse.GetRawResponse().IsError) ? nbaGameEntities : new List<NBAGameEntity>();
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

                Standings.Root output;

                RapidAPIContent content = GetNBAStandingsDataFromRapidAPI(season).Result;

                if (string.IsNullOrEmpty(content.Content)) {
                    return new TeamStatsResponse();
                }

                output = JsonConvert.DeserializeObject<Standings.Root>(content.Content);

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

        // Rapid API Methods
        public async Task<RapidAPIContent> GetNBAStandingsDataFromRapidAPI(string season)
        {
            try {
                HttpClient httpClient = new();
                HttpRequestMessage request = new() {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api-nba-v1.p.rapidapi.com/standings?league=standard&season=" + season),
                    Headers =
                    {
                        { "X-RapidAPI-Key", _appConfig.RapidAPIKey },
                        { "X-RapidAPI-Host", "api-nba-v1.p.rapidapi.com" },
                    },
                };

                HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var nonValidatedHeaders = response.Headers.NonValidated;

                int requestsRemaining = int.Parse(response.Headers.NonValidated["x-ratelimit-requests-remaining"].ElementAt(0));

                if (requestsRemaining == 0) {
                    SetRapidAPITimeout(DateTimeOffset.UtcNow.AddDays(1));
                }

                return new RapidAPIContent() {
                    Content = await response.Content.ReadAsStringAsync(),
                    RequestsRemaining = requestsRemaining
                };
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public async Task<RapidAPIContent> GetNBAGamesDataFromRapidAPI(DateTime date)
        {
            try {
                string dateString = date.ToString("yyyy-MM-dd");

                HttpClient httpClient = new();
                HttpRequestMessage request = new() {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api-nba-v1.p.rapidapi.com/games?date=" + dateString),
                    Headers =
                    {
                        { "X-RapidAPI-Key", _appConfig.RapidAPIKey },
                        { "X-RapidAPI-Host", "api-nba-v1.p.rapidapi.com" },
                    },
                };

                HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                var nonValidatedHeaders = response.Headers.NonValidated;

                int requestsRemaining = int.Parse(response.Headers.NonValidated["x-ratelimit-requests-remaining"].ElementAt(0));

                if (requestsRemaining == 0) {
                    SetRapidAPITimeout(DateTimeOffset.UtcNow.AddDays(1));
                }

                return new RapidAPIContent() {
                    Content = await response.Content.ReadAsStringAsync(),
                    RequestsRemaining = requestsRemaining
                };
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

        public bool SetRapidAPITimeout(DateTimeOffset timeout)
        {
            TimeoutEntity rapidAPITimeout = new() {
                PartitionKey = AppConstants.SysConfig_RapidAPITimeout,
                RowKey = Guid.NewGuid().ToString(),
                NextTimeAvailableDateTime = timeout,
                Timestamp = DateTime.Now
            };

            Response response = _tableStorageHelper.UpsertEntity(rapidAPITimeout, AppConstants.SystemConfigurationTable).Result;

            return (response != null && !response.IsError);
        }

        public bool IsRapidAPIAvailable()
        {
            string filter = TableClient.CreateQueryFilter<TimeoutEntity>((group) => group.PartitionKey == AppConstants.SysConfig_RapidAPITimeout);

            var response = _tableStorageHelper.QueryEntities<TimeoutEntity>(AppConstants.SystemConfigurationTable, filter).Result;

            TimeoutEntity timeoutEntity = response.OrderBy(x => x.NextTimeAvailableDateTime).FirstOrDefault();

            return (timeoutEntity == null || timeoutEntity.NextTimeAvailableDateTime < DateTimeOffset.UtcNow);
        }
    }
}
