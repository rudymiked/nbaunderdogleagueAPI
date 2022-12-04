using Azure;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using Newtonsoft.Json;
using static nbaunderdogleagueAPI.Models.RapidAPI_NBA;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface INBADataAccess
    {
        TeamStatsResponse GetTeamStatsFromRapidAPI();
        List<TeamStats> UpdateTeamStatsFromRapidAPI();
        Task<RapidAPIContent> GetNBAStandingsDataFromRapidAPI(string season);
    }
    public class NBADataAccess : INBADataAccess
    {
        private readonly ILogger _logger;
        private readonly ITableStorageHelper _tableStorageHelper;
        private readonly AppConfig _appConfig;
        public NBADataAccess(IOptions<AppConfig> appConfig, ILogger<NBADataAccess> logger, ITableStorageHelper tableStorageHelper)
        {
            _logger = logger;
            _tableStorageHelper = tableStorageHelper;
            _appConfig = appConfig.Value;
        }

        public List<TeamStats> UpdateTeamStatsFromRapidAPI()
        {
            TeamStatsResponse teamStatsResponse = GetTeamStatsFromRapidAPI();
            List<TeamStats> teamStats = teamStatsResponse.TeamStats.OrderByDescending(team => team.Wins).ToList();
            //int remainingCalls = teamStatsDictionary.Keys.FirstOrDefault(); // XXX

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

        public TeamStatsResponse GetTeamStatsFromRapidAPI()
        {
            try {
                // season starts in October, switch season on site in September
                DateTimeOffset now = DateTimeOffset.UtcNow;
                string season = now.Month >= 9 ? now.Year.ToString() : (now.Year - 1).ToString();

                RapidAPI_NBA.Standings.Root output;

                RapidAPIContent content = GetNBAStandingsDataFromRapidAPI(season).Result;

                if (string.IsNullOrEmpty(content.Content)) {
                    return new TeamStatsResponse();
                }

                output = JsonConvert.DeserializeObject<RapidAPI_NBA.Standings.Root>(content.Content);

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

                //int x = int.Parse(remainingCalls.ElementAt(0));

                return new RapidAPIContent() {
                    Content = await response.Content.ReadAsStringAsync(),
                    RequestsRemaining = requestsRemaining
                };
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }
    }
}
