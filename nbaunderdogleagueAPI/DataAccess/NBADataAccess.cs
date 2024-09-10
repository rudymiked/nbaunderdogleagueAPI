using Azure;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using Newtonsoft.Json;
using System.Linq;
using static nbaunderdogleagueAPI.Models.RapidAPI_NBA.RapidAPI_NBA;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface INBADataAccess
    {
        List<NBAGameEntity> UpdateScoreboardFromRapidAPI();
        Task<RapidAPIContent> GetNBAGamesDataFromRapidAPIBySeason(string season);
        List<TeamStats> UpdateTeamStatsFromRapidAPI(string Year = "");
        List<Scoreboard> NBAScoreboard(string groupId);
        List<PlayoffData> UpdatePlayoffData();
        Dictionary<string, (int PlayoffWins, int ClinchedPlayoffBirth)> GetPlayoffData(string season);
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

        public GameResponse GetGamesFromRapidAPI(string season)
        {
            try {
                Game.Root output;

                RapidAPIContent content = GetNBAGamesDataFromRapidAPIBySeason(season).Result;

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

        public GameResponse GetYesterdaysGamesFromRapidAPI()
        {
            try {
                DateTime now = DateTime.UtcNow;
                DateTime dayBefore = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-1); // get yesterday's games

                Game.Root output;

                RapidAPIContent content = GetNBAGamesDataFromRapidAPIByDate(dayBefore).Result;

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

        public List<TeamStats> UpdateTeamStatsFromRapidAPI(string Year = "")
        {
            if (!_rapidAPIHelper.IsRapidAPIAvailable()) {
                return new List<TeamStats>();
            }

            // season starts in October, switch season on site in September
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string season = now.Month >= 9 ? now.Year.ToString() : (now.Year - 1).ToString();

            season = string.IsNullOrWhiteSpace(Year) ? season : Year;

            TeamStatsResponse teamStatsResponse = GetTeamStatsFromRapidAPI(season);
            List<TeamStats> teamStats = teamStatsResponse.TeamStats.OrderByDescending(team => team.Wins).ToList();

            Dictionary<string, (int PlayoffWins, int ClinchedPlayoffBirth)> playoffDict = GetPlayoffData(season);

            if (teamStats.Count != 0) {
                List<TeamStatsEntity> teamStatsData = new();

                if (teamStats.Count == 30) {
                    foreach (TeamStats teamData in teamStats) {
                        bool teamInplayoffs = playoffDict.TryGetValue($"{teamData.TeamCity} {teamData.TeamName}", out (int PlayoffWins, int ClinchedPlayoffBirth) playoffData);

                        teamStatsData.Add(new TeamStatsEntity() {
                            PartitionKey = season,
                            RowKey = teamData.TeamName,
                            TeamID = teamData.TeamID,
                            TeamCity = teamData.TeamCity,
                            TeamName = teamData.TeamName,
                            Conference = teamData.Conference,
                            Wins = teamData.Wins,
                            PlayoffWins = teamInplayoffs ? playoffData.PlayoffWins : 0,
                            Losses = teamData.Losses,
                            Standing = teamData.Standing,
                            Ratio = teamData.Ratio,
                            Streak = teamData.Streak,
                            ClinchedPlayoffBirth = teamInplayoffs ? playoffData.ClinchedPlayoffBirth : 0,
                            Logo = teamData.Logo,
                            Year = int.TryParse(season, out int n) ? n : 0,
                            ETag = ETag.All,
                            Timestamp = DateTime.Now
                        });
                    }

                    var updateTeamStatsManuallyResponse = _tableStorageHelper.UpsertEntitiesAsync(teamStatsData, AppConstants.TeamStatsTable).Result;

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

                    List<TeamStatsEntity> teamStatsEntity = new();

                    if (currentTeamStats.Count == 30) {
                        currentTeamStats.ForEach(teamData => teamStatsEntity.Add(new TeamStatsEntity() {
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

                        var updateTeamStatsManuallyResponse = _tableStorageHelper.UpsertEntitiesAsync(teamStatsEntity, AppConstants.TeamStatsTable).Result;

                        return (updateTeamStatsManuallyResponse == AppConstants.Success) ? teamStats : new List<TeamStats>();
                    }
                }
            }

            return new List<TeamStats>();
        }

        public List<PlayoffData> UpdatePlayoffData()
        {
            return [];
        }

        public Dictionary<string, (int PlayoffWins, int ClinchedPlayoffBirth)> GetPlayoffData(string season)
        {
            try {

                DateTime seasonDateTime = new(int.Parse(season), 1, 1);

                // playoffs have not started
                if (int.Parse(season) == AppConstants.CurrentNBASeasonYear) {
                    return [];
                }

                // playoffs have not started
                if (int.Parse(season) == AppConstants.CurrentNBASeasonYear + 1 && DateTime.Now.DayOfYear < AppConstants.NBAEndDate.DayOfYear) {
                    return [];
                }

                // collect all game data per season
                GameResponse gameResponse = GetGamesFromRapidAPI(season);

                // filter out games that are only after the playoffs start
                List<Game.Response> games = gameResponse.Games.Where(g => g.date.start.DayOfYear >= AppConstants.NBAEndDate.DayOfYear && g.date.start.Year == seasonDateTime.Year + 1).ToList();
                Dictionary<string, int> teamWins = new();

                PlayoffData.PlayoffDataDict = new();

                foreach(Game.Response game in games) {
                    string winningTeam = game.teams.home.points > game.teams.visitors.points ? game.teams.home.name : game.teams.visitors.name;

                    if (teamWins.ContainsKey(winningTeam)) {
                        teamWins[winningTeam]++;
                    } else {
                        teamWins.Add(winningTeam, 1);
                    }

                    PlayoffData playoffData = new() {
                        TeamName = winningTeam,
                        ClinchedPlayoffBirth = 1,
                        PlayoffWins = teamWins[winningTeam]
                    };

                    PlayoffData.AddToDictionary(playoffData);
                }

                /*
                collect wins for each team
                if a team wins, ClinchedPlayoffs = 1

                return playoff data
                 
                 */

                return PlayoffData.PlayoffDataDict;
            } catch (Exception ex) {
                _logger.LogError(ex, nameof(GetPlayoffData));
            }

            return [];
        }

        // Use the downloaded game data to update playoff games
        // standings data does not include playoff data
        public List<TeamStats> UpdatePlayoffDataUsingTodaysGames()
        {
            // 1. read game data from score board
            // only start doing this after playoffs begin (although, this could realistically replace the standings endpoint)

            // 2. Update teamStats table with new game data (win/loss)

            try {
                // Current nba season and after playoffs start
                if (AppConstants.PlayoffsStarted) {
                    List<NBAGameEntity> nbaGameEntities = NBAGamesOnScoreboard();

                    IEnumerable<NBAGameEntity> todaysGames = nbaGameEntities.Where(x => x.Timestamp.Value.DayOfYear == AppConstants.CurrentDate.DayOfYear);

                    Dictionary<string, TeamStats> teamStatsDict = _teamService.TeamStatsDictionaryFromStorage();

                    List<TeamStats> teamsWithGamesTodayWhoWon = new();

                    foreach (NBAGameEntity game in todaysGames) {
                        teamsWithGamesTodayWhoWon.AddRange(teamStatsDict
                               .Where(x => (x.Key == game.HomeTeam && game.HomeScore > game.VisitorsScore) || (x.Key == game.VisitorsTeam && game.VisitorsScore > game.HomeScore))
                                .Select(kvp => kvp.Value)
                                .ToList());
                    }

                    List<TeamStatsEntity> updatedTeamStatsEntites = new();

                    for(int i=0;i<teamsWithGamesTodayWhoWon.Count; i++) {
                        TeamStats teamData = teamsWithGamesTodayWhoWon[i];

                        if (teamData.LastUpdated.DayOfYear >= AppConstants.CurrentDate.DayOfYear) {
                            // game already updated today
                            break;
                        }

                        int newWin = teamsWithGamesTodayWhoWon.Any(x => x.TeamName == teamData.TeamName) ? 1 : 0;
                        int playoffWins = (int)(teamData.PlayoffWins + newWin);

                        updatedTeamStatsEntites.Add(new TeamStatsEntity() {
                            PartitionKey = "TeamStats",
                            RowKey = teamData.TeamName,
                            TeamID = teamData.TeamID,
                            TeamCity = teamData.TeamCity,
                            TeamName = teamData.TeamName,
                            Conference = teamData.Conference,
                            Wins = teamData.Wins,
                            PlayoffWins = playoffWins,
                            Losses = teamData.Losses,
                            Standing = teamData.Standing,
                            Ratio = teamData.Ratio,
                            Streak = teamData.Streak,
                            ClinchedPlayoffBirth = teamData.ClinchedPlayoffBirth,
                            Logo = teamData.Logo,
                            ETag = ETag.All,
                            Timestamp = DateTime.UtcNow
                        });
                    }

                    var updateTeamStatsManuallyResponse = _tableStorageHelper.UpsertEntitiesAsync(updatedTeamStatsEntites, AppConstants.TeamStatsTable).Result;

                    return (updateTeamStatsManuallyResponse == AppConstants.Success) ? teamsWithGamesTodayWhoWon : new List<TeamStats>();
                }
            } catch (Exception ex) {
                _logger.LogError(ex, nameof(UpdatePlayoffDataUsingTodaysGames));
            }

            return new List<TeamStats>();
        }

        // Only save games from the previous day
        // if there are no games that do, do not overwrite 
        // this data is just for the scoreboard on the UI
        public List<NBAGameEntity> UpdateScoreboardFromRapidAPI()
        {
            // Rapid API request limit has been met
            // do not update
            if (!_rapidAPIHelper.IsRapidAPIAvailable()) {
                return new List<NBAGameEntity>();
            }

            try {
                GameResponse gameResponse = GetYesterdaysGamesFromRapidAPI();
                List<Game.Response> games = gameResponse.Games;

                // replace current games in scoreboard if there are new games
                //  otherwise, keep most recent games
                // keep 10 most recent games for scoreboard
                if (games.Count > 0) {
                    List<NBAGameEntity> currentScoreboard = _tableStorageHelper.QueryEntitiesAsync<NBAGameEntity>(AppConstants.ScoreboardTable)
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

                    var updateGamesResponse = _tableStorageHelper.UpsertEntitiesAsync(nbaGameEntities, AppConstants.ScoreboardTable).Result;

                    return (updateGamesResponse == AppConstants.Success) ? nbaGameEntities : new List<NBAGameEntity>();
                } else {
                    _logger.LogInformation("No new games on: " + DateTime.Now.ToString());
                }
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new List<NBAGameEntity>();
        }

        private TeamStatsResponse GetTeamStatsFromRapidAPI(string season)
        {
            try {
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

        public async Task<RapidAPIContent> GetNBAGamesDataFromRapidAPIByDate(DateTime date)
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

        public async Task<RapidAPIContent> GetNBAGamesDataFromRapidAPIBySeason(string season)
        {
            try {
                string apiURL = "https://api-nba-v1.p.rapidapi.com/games";
                string parameterString = "?season=" + season;

                return await _rapidAPIHelper.QueryRapidAPI(apiURL, parameterString);
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        private List<NBAGameEntity> NBAGamesOnScoreboard()
        {
            return _tableStorageHelper.QueryEntitiesAsync<NBAGameEntity>(AppConstants.ScoreboardTable)
                            .Result
                            .OrderBy(x => x.Timestamp) // oldest dates first
                            .ToList();
        }

        public List<Scoreboard> NBAScoreboard(string groupId = null)
        {
            try {
                List<NBAGameEntity> nbaGameEntities = NBAGamesOnScoreboard();

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
