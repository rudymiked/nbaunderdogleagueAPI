using Azure;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Root = nbaunderdogleagueAPI.Models.NBAModels.Root;
using Team = nbaunderdogleagueAPI.Models.NBAModels.Team;

namespace nbaunderdogleagueAPI.DataAccess
{
    /*
        TeamStats:
        version 0: https://stats.nba.com/, DEPRECATED.
        version 1: https://data.nba.net/prod/v1/current/standings_all.json, does not work after deploying to Azure
        version 2: ManualTeamStats, populated by RapidAPI every 30 mins. 
    */

    public interface ITeamDataAccess
    {
        Dictionary<string, TeamStats> GetTeamStats();
        Task<Dictionary<string, TeamStats>> GetTeamStatsV1();
        Dictionary<string, TeamStats> GetTeamStatsV2();
        List<TeamEntity> GetTeams();
        List<TeamEntity> AddTeams(List<TeamEntity> teamsEntities);
        List<TeamStats> UpdateTeamStatsManually();
        string UpdateTeamPlayoffWins(TeamStats teamStats);
    }
    public class TeamDataAccess : ITeamDataAccess
    {
        private readonly ILogger _logger;
        private readonly AppConfig _appConfig;
        private readonly ITableStorageHelper _tableStorageHelper;
        public TeamDataAccess(IOptions<AppConfig> options, ILogger<TeamDataAccess> logger, ITableStorageHelper tableStorageHelper)
        {
            _appConfig = options.Value;
            _logger = logger;
            _tableStorageHelper = tableStorageHelper;
        }

        public List<TeamEntity> GetTeams()
        {
            try {
                return _tableStorageHelper.QueryEntities<TeamEntity>(AppConstants.TeamsTable).Result.ToList();
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new List<TeamEntity>();
        }

        public List<TeamEntity> AddTeams(List<TeamEntity> teamEntities)
        {
            // query for team ID
            Dictionary<string, TeamStats> teamStats = GetTeamStatsV2(); // V2 is manual data

            for (int i = 0; i < teamEntities.Count; i++) {
                teamEntities[i].ID = teamStats[teamEntities[i].Name].TeamID;
            }

            var response = _tableStorageHelper.UpsertEntities(teamEntities, AppConstants.TeamsTable).Result;

            return (response == AppConstants.Success) ? teamEntities : new List<TeamEntity>();
        }

        private async Task<string> GetNBAStandingsData(string season)
        {
            try {
                string origin = "https://www.nba.com";
                string baseURL = "https://stats.nba.com/";
                string parameters = "stats/leaguestandingsv3?GroupBy=conf&LeagueID=00&Season=" + season + "&SeasonType=Regular%20Season&Section=overall";
                //string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36 Edg/106.0.1370.52";

                HttpClient httpClient = new() {
                    BaseAddress = new Uri(baseURL)
                };

                using (var request = new HttpRequestMessage(HttpMethod.Get, baseURL + parameters)) {
                    request.Headers.Referrer = new Uri(origin);
                    request.RequestUri = new Uri(baseURL + parameters);
                    request.Headers.Add("Origin", origin);
                    request.Headers.Add("Sec-Fetch-Mode", "cors");
                    request.Headers.Add("cache-control", "max-age=0");
                    request.Headers.Add("Sec-Fetch-Site", "same-site");
                    request.Headers.Add("Sec-Fetch-Dest", "empty");
                    request.Headers.Add("accept-encoding", "Accepflate, sdch");
                    request.Headers.Add("Accept-Language", "en");
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("Access-Control-Allow-Origin", origin);
                    request.Headers.Add("User-Agent", "PostmanRuntime/7.4.0");
                    request.Headers.Add("Host", "stats.nba.com");
                    request.Headers.Add("Connection", "keep-alive");
                    request.Headers.Add("X-Version", "1");
                    request.Headers.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);

                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public Dictionary<string, TeamStats> GetTeamStats()
        {
            try {
                // season starts in October, switch season on site in September
                DateTimeOffset now = DateTimeOffset.UtcNow;
                string season = now.Month >= 9 ? string.Concat(now.Year.ToString(), "-", (now.Year + 1).ToString().AsSpan(2))
                                               : string.Concat((now.Year - 1).ToString(), "-", now.Year.ToString().AsSpan(2));

                LeagueStandingsRootObject output;

                string content = GetNBAStandingsData(season).Result;

                if (string.IsNullOrEmpty(content)) {
                    return new Dictionary<string, TeamStats>();
                }

                output = JsonConvert.DeserializeObject<LeagueStandingsRootObject>(content);

                /*
                 * 
                 * "headers":["LeagueID","SeasonID","TeamID","TeamCity","TeamName","TeamSlug","Conference","ConferenceRecord","PlayoffRank","ClinchIndicator","Division","DivisionRecord","DivisionRank","WINS","LOSSES","WinPCT",
                 * "LeagueRank","Record","HOME","ROAD","L10","Last10Home","Last10Road","OT","ThreePTSOrLess","TenPTSOrMore",
                 * "LongHomeStreak","strLongHomeStreak","LongRoadStreak","strLongRoadStreak","LongWinStreak","LongLossStreak","CurrentHomeStreak","strCurrentHomeStreak","CurrentRoadStreak","strCurrentRoadStreak","CurrentStreak","strCurrentStreak",
                 * "ConferenceGamesBack","DivisionGamesBack","ClinchedConferenceTitle","ClinchedDivisionTitle","ClinchedPlayoffBirth","ClinchedPlayIn","EliminatedConference","EliminatedDivision","AheadAtHalf",
                 * "BehindAtHalf","TiedAtHalf","AheadAtThird","BehindAtThird","TiedAtThird","Score100PTS","OppScore100PTS","OppOver500","LeadInFGPCT","LeadInReb","FewerTurnovers","PointsPG","OppPointsPG","DiffPointsPG",
                 * "vsEast","vsAtlantic","vsCentral","vsSoutheast","vsWest","vsNorthwest","vsPacific","vsSouthwest",
                 * "Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec","Score_80_Plus","Opp_Score_80_Plus","Score_Below_80","Opp_Score_Below_80"]
                 * 
                 */

                List<TeamStats> teamStats = output.ExtractTeamStats(season, _logger);

                return teamStats.ToDictionary(team => team.TeamName);
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new Dictionary<string, TeamStats>();
        }

        public async Task<Dictionary<string, TeamStats>> GetTeamStatsV1()
        {
            HttpClient httpClient = new();

            Stream stream = await httpClient.GetStreamAsync(AppConstants.CurrentNBAStandingsJSON);

            using JsonDocument resp = await JsonDocument.ParseAsync(stream);

            Root data = resp.Deserialize<Root>();

            Dictionary<string, TeamStats> currentNBAStandingsDict = new();

            foreach (Team team in data.league.standard.teams) {
                currentNBAStandingsDict.Add(team.teamSitesOnly.teamNickname, new() {
                    TeamID = int.Parse(team.teamId),
                    TeamName = team.teamSitesOnly.teamNickname,
                    TeamCity = team.teamSitesOnly.teamName,
                    Conference = "",
                    Standing = int.Parse(team.confRank),
                    Wins = int.Parse(team.win),
                    Losses = int.Parse(team.loss),
                    Ratio = double.Parse(team.winPctV2),
                    Streak = int.Parse(team.streak),
                    ClinchedPlayoffBirth = string.IsNullOrEmpty(team.clinchedPlayoffsCode) ? 0 : 1,
                    PlayoffWins = 0, // not on API endpoint
                });
            }

            return currentNBAStandingsDict;
        }

        public Dictionary<string, TeamStats> GetTeamStatsV2()
        {
            var response = _tableStorageHelper.QueryEntities<ManualTeamStatsEntity>(AppConstants.ManualTeamStats).Result;

            List<TeamEntity> teams = GetTeams();

            List<ManualTeamStatsEntity> manualTeamStats = response.ToList();
            List<TeamStats> teamStats = new();

            foreach (ManualTeamStatsEntity teamData in manualTeamStats) {
                TeamEntity currentTeam = teams.First(t => t.RowKey == teamData.RowKey);

                teamStats.Add(new TeamStats() {
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
                    ProjectedWin = currentTeam.ProjectedWin,
                    ProjectedLoss = currentTeam.ProjectedLoss,
                    Score = TeamUtils.CalculateTeamScore(currentTeam.ProjectedWin, currentTeam.ProjectedLoss, teamData.Wins, teamData.Losses, teamData.PlayoffWins)
                });
            }

            return teamStats.ToDictionary(team => team.TeamName);
        }

        public List<TeamStats> UpdateTeamStatsManually()
        {
            List<TeamStats> teamStats = GetTeamStats().Values.OrderByDescending(team => team.Wins).ToList();
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
                ETag = ETag.All,
                Timestamp = DateTime.Now
            }));

            if (teamStats.Count != 0) {
                var updateTeamStatsManuallyResponse = _tableStorageHelper.UpsertEntities(manualTeamStats, AppConstants.ManualTeamStats).Result;

                return (updateTeamStatsManuallyResponse == AppConstants.Success) ? teamStats : new List<TeamStats>();
            } else {
                return new List<TeamStats>();
            }
        }

        public string UpdateTeamPlayoffWins(TeamStats teamStats)
        {
            try {
                var response = _tableStorageHelper.QueryEntities<ManualTeamStatsEntity>(AppConstants.ManualTeamStats).Result;

                List<ManualTeamStatsEntity> manualTeamStats = response.ToList();

                ManualTeamStatsEntity currentTeam = manualTeamStats.Find(x => x.TeamName == teamStats.TeamName);

                if (currentTeam == null || string.IsNullOrWhiteSpace(currentTeam.TeamName)) {
                    return "Team: " + teamStats.TeamName + " does not exist";
                }

                currentTeam.ClinchedPlayoffBirth = 1;
                currentTeam.PlayoffWins = teamStats.PlayoffWins;

                var updateTeamStatsManuallyResponse = _tableStorageHelper.UpsertEntities(manualTeamStats, AppConstants.ManualTeamStats).Result;

                return updateTeamStatsManuallyResponse;
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
