using Dapper;
using System.Data.Common;

namespace EcoSQL.Request;

public static partial class Select
{
    extension(DbProviderFactory dbProviderFactory)
    {
        public bool SelectCollection<TItem>(
            string connectionString,
            string queryText,
            CancellationToken cancellationToken,
            int commandTimeout,
            ResultCollectionUnit<TItem> resultCollectionInfoUnit,
            string? queryAdditionalInfo = null)
        {
            return dbProviderFactory.SelectData(
                connectionString,
                queryText,
                cancellationToken,
                commandTimeout,
                dbProviderFactory.QueryCollection<TItem>,
                resultCollectionInfoUnit,
                getCount: collection => collection.Count,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        public async Task<bool> SelectCollectionAsync<TItem>(
            string connectionString,
            string queryText,
            CancellationToken cancellationToken,
            int commandTimeout,
            ResultCollectionUnit<TItem> resultCollectionInfoUnit,
            string? queryAdditionalInfo = null)
        {
            return await dbProviderFactory.SelectDataAsync(
                connectionString,
                queryText,
                cancellationToken,
                commandTimeout,
                dbProviderFactory.QueryCollectionAsync<TItem>,
                resultCollectionInfoUnit,
                getCount: collection => collection.Count,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        private ICollection<TItem>? QueryCollection<TItem>(
            string connectionString,
            string queryText,
            CancellationToken cancellationToken,
            int commandTimeout)
        {
            using DbDataSource dataSource =
                dbProviderFactory.CreateDataSource(connectionString);
            cancellationToken.ThrowIfCancellationRequested();
            CommandDefinition command = new(
                queryText,
                commandTimeout: commandTimeout,
                cancellationToken: cancellationToken);
            ICollection<TItem> resultCollection = [.. dataSource.CreateConnection()
                .Query<TItem>(command)];
            return resultCollection;
        }

        private async Task<ICollection<TItem>?> QueryCollectionAsync<TItem>(
            string connectionString,
            string queryText,
            CancellationToken cancellationToken,
            int commandTimeout)
        {
            await using DbDataSource dataSource =
                dbProviderFactory.CreateDataSource(connectionString);
            cancellationToken.ThrowIfCancellationRequested();
            CommandDefinition command = new(
                queryText,
                commandTimeout: commandTimeout,
                cancellationToken: cancellationToken);
            ICollection<TItem> resultCollection = [.. await dataSource.CreateConnection()
                .QueryAsync<TItem>(command)];
            return resultCollection;
        }
    }
}