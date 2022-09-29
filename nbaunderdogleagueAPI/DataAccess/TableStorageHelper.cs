using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.DataAccess
{
    public interface ITableStorageHelper
    {
        Task<Response<IReadOnlyList<Response>>> UpsertEntities<T>(List<T> entities, string table) where T : ITableEntity, new();
        Task<Pageable<T>> QueryEntities<T>(string table, string WhereFilter = null) where T : class, ITableEntity, new();
        Task<Response> UpsertEntity<T>(T entity, string table) where T : ITableEntity, new();
    }

    public class TableStorageHelper : ITableStorageHelper
    {
        private readonly AppConfig _appConfig;
        private readonly ILogger _logger;
        public TableStorageHelper(IOptions<AppConfig> options, ILogger<TableStorageHelper> logger)
        {
            _appConfig = options.Value;
            _logger = logger;
        }

        public async Task<Response> UpsertEntity<T>(T entity, string table) where T : ITableEntity, new()
        {
            try {
                TableClient tableClient = new(_appConfig.TableConnection, table);
                await tableClient.CreateIfNotExistsAsync();

                return tableClient.UpsertEntity<T>(entity, TableUpdateMode.Replace);
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public async Task<Response<IReadOnlyList<Response>>> UpsertEntities<T>(List<T> entities, string table) where T : ITableEntity, new()
        {
            try {
                TableClient tableClient = new(_appConfig.TableConnection, table);
                await tableClient.CreateIfNotExistsAsync();

                List<TableTransactionAction> addBatch = new();

                addBatch.AddRange(entities.Select(f => new TableTransactionAction(TableTransactionActionType.UpsertMerge, f)));

                return await tableClient.SubmitTransactionAsync(addBatch);
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public async Task<Pageable<T>> QueryEntities<T>(string table, string WhereFilter = null) where T : class, ITableEntity, new()
        {
            try {
                TableClient tableClient = new(_appConfig.TableConnection, table);
                await tableClient.CreateIfNotExistsAsync();

                return tableClient.Query<T>(WhereFilter);
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }
    }
}
