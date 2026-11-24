using System.Data;
using System.Data.Common;

namespace EcoSQL.Request;

public static partial class Select
{
    extension(DbProviderFactory dbProviderFactory)
    {
        /// <summary>Selects a data table from the database.</summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">SQL query text.</param>
        /// <param name="resultTableInfoUnit">Query execution information unit.</param>
        /// <param name="commandTimeout">Query command timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="queryAdditionalInfo">Additional query information.</param>
        /// <returns>True if the data table was obtained successfully</returns>
        public bool SelectDataTable(
            string connectionString,
            string queryText,
            ResultTableUnit resultTableInfoUnit,
            int commandTimeout,
            CancellationToken cancellationToken,
            string? queryAdditionalInfo = null)
        {
            return SelectData(
                dbProviderFactory.QueryDataTable,
                connectionString,
                queryText,
                resultTableInfoUnit,
                commandTimeout,
                cancellationToken,
                //getCount: dataTable => dataTable.Rows.Count,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        /// <summary>Selects a data table from the database.</summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryInfo">Query information.</param>
        /// <param name="resultTableInfoUnit">Query execution information unit.</param>
        /// <param name="queryAdditionalInfo">Additional query information.</param>
        /// <returns>True if the data table was obtained successfully.</returns>
        public bool SelectDataTable(
            string connectionString,
            QueryInfo queryInfo,
            ResultTableUnit resultTableInfoUnit,
            string? queryAdditionalInfo = null)
        {
            if (!queryInfo.IsValid(resultTableInfoUnit))
                return false;

            return dbProviderFactory.SelectDataTable(
                connectionString,
                queryInfo.Text,
                resultTableInfoUnit,
                queryInfo.CommandTimeout,
                queryInfo.CancellationToken ?? CancellationToken.None,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        /// <summary>Asynchronously selects a data table from the database.</summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">SQL query text.</param>
        /// <param name="resultTableInfoUnit">Query execution information unit.</param>
        /// <param name="commandTimeout">Query command timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="queryAdditionalInfo">Additional query information.</param>
        /// <returns>True if the data table was obtained successfully.</returns>
        public async Task<bool> SelectDataTableAsync(
            string connectionString,
            string queryText,
            ResultTableUnit resultTableInfoUnit,
            int commandTimeout,
            CancellationToken cancellationToken,
            string? queryAdditionalInfo = null)
        {
            return await SelectDataAsync(
                dbProviderFactory.QueryDataTableAsync,
                connectionString,
                queryText,
                resultTableInfoUnit,
                commandTimeout,
                cancellationToken,
                //getCount: dataTable => dataTable.Rows.Count,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        /// <summary>Asynchronously selects a data table from the database.</summary>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryInfo">Query information.</param>
        /// <param name="resultTableInfoUnit">Query execution information unit.</param>
        /// <param name="queryAdditionalInfo">Additional query information.</param>
        /// <returns>True if the data table was obtained successfully.</returns>
        public async Task<bool> SelectDataTableAsync(
            string connectionString,
            QueryInfo queryInfo,
            ResultTableUnit resultTableInfoUnit,
            string? queryAdditionalInfo = null)
        {
            if (!queryInfo.IsValid(resultTableInfoUnit))
                return false;

            return await dbProviderFactory.SelectDataTableAsync(
                connectionString,
                queryInfo.Text,
                resultTableInfoUnit,
                queryInfo.CommandTimeout,
                queryInfo.CancellationToken ?? CancellationToken.None,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        /// <summary>Queries a data table from the database.</summary>
        /// <remarks>This method can be used as a <see cref="QueryData{T}"/> delegate
        /// with a <see cref="DataTable"/> return type.</remarks>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">SQL query text.</param>
        /// <param name="commandTimeout">Command timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The queried data table, or null if the operation failed.</returns>
        public DataTable? QueryDataTable(
            string connectionString,
            string queryText,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            using DbConnection? connection = dbProviderFactory.CreateConnection();
            using DbCommand? queryCommand = dbProviderFactory.CreateCommand();
            using DbDataAdapter? dataAdapter = dbProviderFactory.CreateDataAdapter();

            if (connection is null || queryCommand is null || dataAdapter is null)
                return null;

            connection.ConnectionString = connectionString;
            queryCommand.Connection = connection;
            queryCommand.CommandText = queryText;
            queryCommand.CommandTimeout = commandTimeout;
            dataAdapter.SelectCommand = queryCommand;
            cancellationToken.ThrowIfCancellationRequested();

            DataTable resultTable = new();
            dataAdapter.Fill(resultTable);
            return resultTable;
        }

        /// <summary>Asynchronously queries a data table from the database.</summary>
        /// <remarks>This method can be used as a <see cref="QueryDataAsync{T}"/> delegate
        /// with a <see cref="DataTable"/> return type.</remarks>
        /// <param name="connectionString">Connection string.</param>
        /// <param name="queryText">SQL query text.</param>
        /// <param name="commandTimeout">Command timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The queried data table, or null if the operation failed.</returns>
        public async Task<DataTable?> QueryDataTableAsync(
            string connectionString,
            string queryText,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            await using DbConnection? connection = dbProviderFactory.CreateConnection();
            await using DbCommand? queryCommand = dbProviderFactory.CreateCommand();

            if (connection is null || queryCommand is null)
                return null;

            connection.ConnectionString = connectionString;
            queryCommand.Connection = connection;
            queryCommand.CommandText = queryText;
            queryCommand.CommandTimeout = commandTimeout;
            cancellationToken.ThrowIfCancellationRequested();

            await using DbDataReader reader =
                await queryCommand.ExecuteReaderAsync(cancellationToken);
            DataTable resultTable = new();
            resultTable.Load(reader);
            return resultTable;
        }
    }
}
