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
        /// <param name="resultCollectionInfoUnit">Query execution information unit</param>
        /// <param name="commandTimeout">Query command timeout</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="queryAdditionalInfo">Additional query information</param>
        /// <returns>True if the collection was obtained successfully</returns>
        public bool SelectCollection<TItem>(
            string connectionString,
            string queryText,
            ResultCollectionUnit<TItem> resultCollectionInfoUnit,
            int commandTimeout,
            CancellationToken cancellationToken,
            string? queryAdditionalInfo = null)
        {
            return SelectData(
                dbProviderFactory.QueryCollection<TItem>,
                connectionString,
                queryText,
                resultCollectionInfoUnit,
                commandTimeout,
                cancellationToken,
                getCount: collection => collection.Count,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        /// <summary>Asynchronously selects a collection of items from the database</summary>
        /// <typeparam name="TItem">Type of the items in the collection</typeparam>
        /// <param name="connectionString">Connection string</param>
        /// <param name="queryText">SQL query text</param>
        /// <param name="resultCollectionInfoUnit">Query execution information unit</param>
        /// <param name="commandTimeout">Query command timeout</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="queryAdditionalInfo">Additional query information</param>
        /// <returns>True if the collection was obtained successfully</returns>
        public async Task<bool> SelectCollectionAsync<TItem>(
            string connectionString,
            string queryText,
            ResultCollectionUnit<TItem> resultCollectionInfoUnit,
            int commandTimeout,
            CancellationToken cancellationToken,
            string? queryAdditionalInfo = null)
        {
            return await SelectDataAsync(
                dbProviderFactory.QueryCollectionAsync<TItem>,
                connectionString,
                queryText,
                resultCollectionInfoUnit,
                commandTimeout,
                cancellationToken,
                getCount: collection => collection.Count,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        private ICollection<TItem>? QueryCollection<TItem>(
            string connectionString,
            string queryText,
            int commandTimeout,
            CancellationToken cancellationToken)
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
            int commandTimeout,
            CancellationToken cancellationToken)
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