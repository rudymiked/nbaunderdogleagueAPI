using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IUserDataAccess
    {
        List<UserEntity> GetUsers(string leagueId);
        User AddUser(User user);
    }
    public class UserDataAccess : IUserDataAccess
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        private readonly ITableStorageHelper _tableStorageHelper;
        public UserDataAccess(IOptions<AppConfig> options, ILogger<UserDataAccess> logger, ITableStorageHelper tableStorageHelper)
        {
            _appConfig = options.Value;
            _logger = logger;
            _tableStorageHelper = tableStorageHelper;
        }
        public User AddUser(User user)
        {
            UserEntity entity = new() {
                PartitionKey = user.League.ToString(),
                RowKey = user.Email,
                Email = user.Email,
                League = user.League,
                ETag = ETag.All,
                Timestamp = DateTime.Now
            };

            var response = _tableStorageHelper.UpsertEntity(entity, AppConstants.UsersTable).Result;

            return (response != null && !response.IsError) ? user : new User();
        }

        public List<UserEntity> GetUsers(string leagueId)
        {
            string filter = TableClient.CreateQueryFilter<UserEntity>((user) => user.PartitionKey == leagueId);

            var response = _tableStorageHelper.QueryEntities<UserEntity>(AppConstants.UsersTable, filter).Result;

            return response.ToList();
        }
    }
}
