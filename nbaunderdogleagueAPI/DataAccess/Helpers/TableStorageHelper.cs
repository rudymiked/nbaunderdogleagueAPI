using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using nbaunderdogleagueAPI.Models;
using System.ComponentModel;

namespace nbaunderdogleagueAPI.DataAccess.Helpers
{
    public interface ITableStorageHelper
    {
        Task<Pageable<T>> QueryEntitiesAsync<T>(string table, string WhereFilter = null) where T : class, ITableEntity, new();
        Task<Response> UpsertEntityAsync<T>(T entity, string table) where T : ITableEntity, new();
        Task<string> UpsertEntitiesAsync<T>(List<T> entities, string table) where T : ITableEntity, new();
        Task<Response> UpdateEntityAsync<T>(T entity, string table) where T : ITableEntity, new();
        Task<Response> DeleteEntityAsync<T>(T entity, string table) where T : ITableEntity, new();
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

        public async Task<Response> DeleteEntityAsync<T>(T entity, string table) where T : ITableEntity, new()
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
                    var deleteEntity = DeleteEntityAsync(entity, table);
                } catch (Exception ex) {
                    _logger.LogError(ex, ex.Message);
                }
            }

            return Task.CompletedTask;
        }

        public async Task<Response> UpdateEntityAsync<T>(T entity, string table) where T : ITableEntity, new()
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

        public async Task<Response> UpsertEntityAsync<T>(T entity, string table) where T : ITableEntity, new()
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

        public async Task<string> UpsertEntitiesAsync<T>(List<T> entities, string table) where T : ITableEntity, new()
        {
            try {
                TableClient tableClient = new(_appConfig.TableConnection, table);
                await tableClient.CreateIfNotExistsAsync();

                List<TableTransactionAction> addBatch = new();

                for (int i = 0; i < entities.Count; i += AppConstants.MaxBatchSizeAzureTableStorage) {
                    IEnumerable<T> batch = entities.Skip(i).Take(AppConstants.MaxBatchSizeAzureTableStorage);
                    addBatch.AddRange(batch.Select(f => new TableTransactionAction(TableTransactionActionType.UpsertMerge, f)));

                    Response<IReadOnlyList<Response>> response = await tableClient.SubmitTransactionAsync(addBatch);
                    Response rawResponse = response.GetRawResponse();

                    // something went wrong
                    if (rawResponse.IsError) {
                        return rawResponse.ReasonPhrase;
                    }

                    addBatch.Clear();
                }

                return AppConstants.Success;
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }

            return null;
        }


        public async Task<Pageable<T>> QueryEntitiesAsync<T>(string table, string WhereFilter = null) where T : class, ITableEntity, new()
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
