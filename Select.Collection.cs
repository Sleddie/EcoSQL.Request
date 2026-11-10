using Dapper;
using System.Data.Common;

namespace EcoSQL.Request;

public static partial class Select
{
    extension(DbProviderFactory dbProviderFactory)
    {
        /// <summary>Selects a collection of items from the database</summary>
        /// <typeparam name="TItem">Type of the items in the collection</typeparam>
        /// <param name="connectionString">Connection string</param>
        /// <param name="queryText">SQL query text</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="commandTimeout">Query command timeout</param>
        /// <param name="resultCollectionInfoUnit">Query execution information unit</param>
        /// <param name="queryAdditionalInfo">Additional query information</param>
        /// <returns>True if the collection was obtained successfully</returns>
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

        /// <summary>Asynchronously selects a collection of items from the database</summary>
        /// <typeparam name="TItem">Type of the items in the collection</typeparam>
        /// <param name="connectionString">Connection string</param>
        /// <param name="queryText">SQL query text</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="commandTimeout">Query command timeout</param>
        /// <param name="resultCollectionInfoUnit">Query execution information unit</param>
        /// <param name="queryAdditionalInfo">Additional query information</param>
        /// <returns>True if the collection was obtained successfully</returns>
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