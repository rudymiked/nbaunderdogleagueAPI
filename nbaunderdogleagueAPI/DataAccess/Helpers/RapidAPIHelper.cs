using Azure;
using Azure.Data.Tables;
using nbaunderdogleagueAPI.Models;
using static nbaunderdogleagueAPI.Models.RapidAPI_NBA.RapidAPI_NBA;

namespace nbaunderdogleagueAPI.DataAccess.Helpers
{
    public interface IRapidAPIHelper
    {
        bool SetRapidAPITimeout(DateTimeOffset timeout);
        bool IsRapidAPIAvailable();
    }
    public class RapidAPIHelper : IRapidAPIHelper
    {
        private readonly ITableStorageHelper _tableStorageHelper;
        public RapidAPIHelper(ITableStorageHelper tableStorageHelper)
        {
            _tableStorageHelper = tableStorageHelper;
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
