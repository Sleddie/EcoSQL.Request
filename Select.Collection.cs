using Dapper;
using System.Data.Common;

namespace EcoSQL.Request;

public static partial class Select
{
    extension(DbProviderFactory dbProviderFactory)
    {
        /// <summary>Selects a collection of items from the database.</summary>
        /// <typeparam name="TItem">Type of the items in the collection.</typeparam>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">SQL query text.</param>
        /// <param name="resultCollectionInfoUnit">Query execution information unit.</param>
        /// <param name="commandTimeout">Query command timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="queryAdditionalInfo">Additional query information.</param>
        /// <returns>True if the collection was obtained successfully.</returns>
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

        /// <summary>Selects a collection of items from the database.</summary>
        /// <typeparam name="TItem">Type of the items in the collection.</typeparam>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryInfo">Query information.</param>
        /// <param name="resultCollectionInfoUnit">Query execution information unit.</param>
        /// <param name="queryAdditionalInfo">Additional query information.</param>
        /// <returns>True if the collection was obtained successfully.</returns>
        public bool SelectCollection<TItem>(
            string connectionString,
            QueryInfo queryInfo,
            ResultCollectionUnit<TItem> resultCollectionInfoUnit,
            string? queryAdditionalInfo = null)
        {
            if (!queryInfo.IsValid(resultCollectionInfoUnit))
                return false;

            return dbProviderFactory.SelectCollection(
                connectionString,
                queryInfo.Text,
                resultCollectionInfoUnit,
                queryInfo.CommandTimeout,
                queryInfo.CancellationToken ?? CancellationToken.None,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        /// <summary>Asynchronously selects a collection of items from the database.</summary>
        /// <typeparam name="TItem">Type of the items in the collection.</typeparam>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">SQL query text.</param>
        /// <param name="resultCollectionInfoUnit">Query execution information unit.</param>
        /// <param name="commandTimeout">Query command timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="queryAdditionalInfo">Additional query information.</param>
        /// <returns>True if the collection was obtained successfully.</returns>
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

        /// <summary>Asynchronously selects a collection of items from the database.</summary>
        /// <typeparam name="TItem">Type of the items in the collection.</typeparam>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryInfo">Query information.</param>
        /// <param name="resultCollectionInfoUnit">Query execution information unit.</param>
        /// <param name="queryAdditionalInfo">Additional query information.</param>
        /// <returns>True if the collection was obtained successfully.</returns>
        public async Task<bool> SelectCollectionAsync<TItem>(
            string connectionString,
            QueryInfo queryInfo,
            ResultCollectionUnit<TItem> resultCollectionInfoUnit,
            string? queryAdditionalInfo = null)
        {
            if (!queryInfo.IsValid(resultCollectionInfoUnit))
                return false;

            return await dbProviderFactory.SelectCollectionAsync(
                connectionString,
                queryInfo.Text,
                resultCollectionInfoUnit,
                queryInfo.CommandTimeout,
                queryInfo.CancellationToken ?? CancellationToken.None,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        /// <summary>Queries a collection of items from the database.</summary>
        /// <remarks>This method can be used as a <see cref="QueryData{T}"/> delegate
        /// with a <see cref="ICollection{T}"/> return type.</remarks>
        /// <typeparam name="TItem">Type of the items in the collection.</typeparam>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">SQL query text.</param>
        /// <param name="commandTimeout">Command timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The queried collection of items, or null if the operation failed.</returns>
        public ICollection<TItem>? QueryCollection<TItem>(
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

        /// <summary>Asynchronously queries a collection of items from the database.</summary>
        /// <remarks>This method can be used as a <see cref="QueryDataAsync{T}"/> delegate
        /// with a <see cref="ICollection{T}"/> return type.</remarks>
        /// <typeparam name="TItem">Type of the items in the collection.</typeparam>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">SQL query text.</param>
        /// <param name="commandTimeout">Command timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The queried collection of items, or null if the operation failed.</returns>
        public async Task<ICollection<TItem>?> QueryCollectionAsync<TItem>(
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