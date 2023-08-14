using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using static nbaunderdogleagueAPI.Models.RapidAPI_NBA.RapidAPI_NBA;

namespace nbaunderdogleagueAPI.DataAccess.Helpers
{
    public interface IRapidAPIHelper
    {
        public bool SetRapidAPITimeout(DateTimeOffset timeout);
        public bool IsRapidAPIAvailable();
        public Task<RapidAPIContent> QueryRapidAPI(string apiUrl, string parameterString);

    }
    public class RapidAPIHelper : IRapidAPIHelper
    {
        private readonly ITableStorageHelper _tableStorageHelper;
        private readonly ILogger _logger;
        private readonly AppConfig _appConfig;
        public RapidAPIHelper(IOptions<AppConfig> appConfig, ITableStorageHelper tableStorageHelper, ILogger<RapidAPIHelper> logger)
        {
            _tableStorageHelper = tableStorageHelper;
            _logger = logger;
            _appConfig = appConfig.Value;
        }

        public bool SetRapidAPITimeout(DateTimeOffset timeout)
        {
            TimeoutEntity rapidAPITimeout = new() {
                PartitionKey = AppConstants.SysConfig_RapidAPITimeout,
                RowKey = Guid.NewGuid().ToString(),
                NextTimeAvailableDateTime = timeout.AddHours(1),
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

        public async Task<RapidAPIContent> QueryRapidAPI(string apiUrl, string parameterString)
        {
            try {
                HttpClient httpClient = new();
                HttpRequestMessage request = new() {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(apiUrl + parameterString),
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
    }
}
