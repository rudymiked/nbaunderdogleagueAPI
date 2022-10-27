using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface ITeamDataAccess
    {
        Dictionary<string, TeamStats> GetTeamStats();
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

        public Dictionary<string, TeamStats> GetTeamStats()
        {
            try {
                string season = "2022-23";
                string origin = "https://www.nba.com";
                string baseURL = "https://stats.nba.com/";
                string parameters = "stats/leaguestandingsv3?GroupBy=conf&LeagueID=00&Season=" + season + "&SeasonType=Regular%20Season&Section=overall";
                string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36 Edg/106.0.1370.52";

                _logger.LogError(baseURL + parameters);

                HttpClient httpClient = new() {
                    BaseAddress = new Uri(baseURL)
                };

                httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
                httpClient.DefaultRequestHeaders.Add("Origin", origin);
                httpClient.DefaultRequestHeaders.Add("Referer", origin);
                httpClient.DefaultRequestHeaders.Add("Host", "stats.nba.com");
                httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                string content = httpClient.GetStringAsync(baseURL + parameters).Result;

                LeagueStandingsRootObject output = JsonConvert.DeserializeObject<LeagueStandingsRootObject>(content);

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

                Dictionary<string, TeamStats> teamStatsDict = new();

                teamStats.ForEach(team => teamStatsDict.Add(team.TeamName, team));

                return teamStatsDict;
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return new Dictionary<string, TeamStats>();
        }
    }
}
