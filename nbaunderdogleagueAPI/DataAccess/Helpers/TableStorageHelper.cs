using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;

namespace nbaunderdogleagueAPI.DataAccess.Helpers
{
    public interface ITableStorageHelper
    {
        Task<Pageable<T>> QueryEntities<T>(string table, string WhereFilter = null) where T : class, ITableEntity, new();
        Task<Response> UpsertEntity<T>(T entity, string table) where T : ITableEntity, new();
        Task<Response<IReadOnlyList<Response>>> UpsertEntities<T>(List<T> entities, string table) where T : ITableEntity, new();
        Task<Response> UpdateEntity<T>(T entity, string table) where T : ITableEntity, new();
        Task<Response> DeleteEntity<T>(T entity, string table) where T : ITableEntity, new();
        Task DeleteAllEntities<T>(List<T> entities, string table) where T : ITableEntity, new();
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

        public async Task<Response> DeleteEntity<T>(T entity, string table) where T : ITableEntity, new()
        {
            try {
                TableClient tableClient = new(_appConfig.TableConnection, table);
                await tableClient.CreateIfNotExistsAsync();

                return tableClient.DeleteEntity(entity.PartitionKey, entity.RowKey);
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public Task DeleteAllEntities<T>(List<T> entities, string table) where T : ITableEntity, new()
        {
            foreach (T entity in entities) {
                try {
                    var deleteEntity = DeleteEntity(entity, table);
                }
                catch (Exception ex) {
                    _logger.LogError(ex, ex.Message);
                }
            }

            return Task.CompletedTask;
        }

        public async Task<Response> UpdateEntity<T>(T entity, string table) where T : ITableEntity, new()
        {
            try {
                TableClient tableClient = new(_appConfig.TableConnection, table);
                await tableClient.CreateIfNotExistsAsync();

                return tableClient.UpdateEntity(entity, entity.ETag, TableUpdateMode.Merge);
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }

        public async Task<Response> UpsertEntity<T>(T entity, string table) where T : ITableEntity, new()
        {
            try {
                TableClient tableClient = new(_appConfig.TableConnection, table);
                await tableClient.CreateIfNotExistsAsync();

                return tableClient.UpsertEntity(entity, TableUpdateMode.Merge);
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
