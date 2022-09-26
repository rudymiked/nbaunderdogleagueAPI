using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IUserDataAccess
    {
        List<UserEntity> GetUsers();
        List<UserEntity> AddUsers(List<UserEntity> userEntities);
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
        public List<UserEntity> AddUsers(List<UserEntity> userEntities)
        {
            var response = _tableStorageHelper.UpsertEntities(userEntities, AppConstants.UsersTable).Result;

            return (response != null && !response.GetRawResponse().IsError) ? userEntities : new List<UserEntity>();
        }

        public List<UserEntity> GetUsers()
        {
            var response = _tableStorageHelper.QueryEntities<UserEntity>(AppConstants.UsersTable).Result;

            return response.ToList();
        }
    }
}
