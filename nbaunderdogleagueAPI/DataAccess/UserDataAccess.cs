using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.DataAccess.Helpers;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IUserDataAccess
    {
        List<UserEntity> GetUsers(string groupId);
        User UpsertUser(User user);
        User UpdateUser(User user);
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

        public User UpsertUser(User user)
        {
            if (Guid.Parse(user.Group) != Guid.Empty && user.Email != string.Empty) {
                UserEntity entity = new() {
                    PartitionKey = user.Group.ToString(),
                    RowKey = user.Email,
                    Email = user.Email,
                    Group = Guid.Parse(user.Group),
                    Team = user.Team,
                    Username = user.Username,
                    Timestamp = DateTime.Now,
                    ETag = ETag.All
                };

                Response response = _tableStorageHelper.UpsertEntity(entity, AppConstants.UsersTable).Result;

                return (response != null && !response.IsError) ? user : new User();
            }

            return new User();
        }

        public User UpdateUser(User user)
        {
            if (Guid.Parse(user.Group) != Guid.Empty && user.Email != string.Empty) {
                UserEntity entity = new() {
                    PartitionKey = user.Group.ToString(),
                    RowKey = user.Email,
                    Email = user.Email,
                    Group = Guid.Parse(user.Group),
                    Team = !string.IsNullOrEmpty(user.Team) ? user.Team : null,
                    Username = !string.IsNullOrEmpty(user.Username) ? user.Username : null,
                    Timestamp = DateTime.Now,
                    ETag = ETag.All
                };

                var response = _tableStorageHelper.UpdateEntity(entity, AppConstants.UsersTable).Result;

                return (response != null && !response.IsError) ? user : new User();
            }

            return new User();
        }

        public List<UserEntity> GetUsers(string groupId)
        {
            if (groupId != null && groupId != string.Empty) {
                string filter = TableClient.CreateQueryFilter<UserEntity>((user) => user.PartitionKey == groupId);

                var response = _tableStorageHelper.QueryEntities<UserEntity>(AppConstants.UsersTable, filter).Result;

                return response.ToList();
            }

            return new List<UserEntity>();
        }
    }
}
