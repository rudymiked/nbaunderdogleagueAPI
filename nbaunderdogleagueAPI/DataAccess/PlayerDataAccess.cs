using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;
using nbaunderdogleagueAPI.Services;
using Newtonsoft.Json;
using static nbaunderdogleagueAPI.Models.RapidAPI_NBA.RapidAPI_NBA;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IPlayerDataAccess
    {
        //public GameResponse GetPlayerStatsFromRapidAPI();
        //public Task<RapidAPIContent> GetPlayerStatssDataFromRapidAPI(DateTime date);
    }
    public class PlayerDataAccess : IPlayerDataAccess
    {
        private readonly ILogger _logger;
        private readonly ITableStorageHelper _tableStorageHelper;
        private readonly AppConfig _appConfig;
        public PlayerDataAccess(IOptions<AppConfig> appConfig, ILogger<NBADataAccess> logger, ITableStorageHelper tableStorageHelper)
        {
            _logger = logger;
            _tableStorageHelper = tableStorageHelper;
            _appConfig = appConfig.Value;
        }

        //public GameResponse GetPlayerStatsFromRapidAPI()
        //{
        //    try {
        //        DateTime now = DateTime.UtcNow;
        //        DateTime dayBefore = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(-1); // get yesterday's games

        //        Game.Root output;

        //        RapidAPIContent content = GetNBAGamesDataFromRapidAPI(dayBefore).Result; // XXX

        //        if (string.IsNullOrEmpty(content.Content)) {
        //            return new GameResponse();
        //        }

        //        output = JsonConvert.DeserializeObject<Game.Root>(content.Content);

        //        List<Game.Response> games = output.response;

        //        return new GameResponse() {
        //            Games = games,
        //            RequestsRemaining = content.RequestsRemaining
        //        };
        //    } catch (Exception ex) {
        //        _logger.LogError(ex, ex.Message);
        //    }

        //    return new GameResponse();
        //}

        //public async Task<RapidAPIContent> GetPlayerStatssDataFromRapidAPI(DateTime date)
        //{
        //    try {
        //        string dateString = date.ToString("yyyy-MM-dd");

        //        HttpClient httpClient = new();
        //        HttpRequestMessage request = new() {
        //            Method = HttpMethod.Get,
        //            RequestUri = new Uri("https://api-nba-v1.p.rapidapi.com/games?date=" + dateString),
        //            Headers =
        //            {
        //                { "X-RapidAPI-Key", _appConfig.RapidAPIKey },
        //                { "X-RapidAPI-Host", "api-nba-v1.p.rapidapi.com" },
        //            },
        //        };

        //        HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);

        //        response.EnsureSuccessStatusCode();

        //        var nonValidatedHeaders = response.Headers.NonValidated;

        //        int requestsRemaining = int.Parse(response.Headers.NonValidated["x-ratelimit-requests-remaining"].ElementAt(0));

        //        if (requestsRemaining == 0) {
        //            _rapidAPIHelper.SetRapidAPITimeout(DateTimeOffset.UtcNow.AddDays(1));
        //        }

        //        return new RapidAPIContent() {
        //            Content = await response.Content.ReadAsStringAsync(),
        //            RequestsRemaining = requestsRemaining
        //        };
        //    } catch (Exception ex) {
        //        _logger.LogError(ex, ex.Message);
        //    }

        //    return null;
        //}
    }
}
