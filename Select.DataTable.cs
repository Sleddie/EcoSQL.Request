using System.Data;
using System.Data.Common;

namespace EcoSQL.Request;

public static partial class Select
{
    extension(DbProviderFactory dbProviderFactory)
    {
        /// <summary>Selects a data table from the database</summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="queryText">SQL query text</param>
        /// <param name="resultTableInfoUnit">Query execution information unit</param>
        /// <param name="commandTimeout">Query command timeout</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="queryAdditionalInfo">Additional query information</param>
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
                getCount: dataTable => dataTable.Rows.Count,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        /// <summary>Asynchronously selects a data table from the database</summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="queryText">SQL query text</param>
        /// <param name="resultTableInfoUnit">Query execution information unit</param>
        /// <param name="commandTimeout">Query command timeout</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="queryAdditionalInfo">Additional query information</param>
        /// <returns>True if the data table was obtained successfully</returns>
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
                getCount: dataTable => dataTable.Rows.Count,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        private DataTable? QueryDataTable(
            string connectionString,
            string queryText,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            using DbDataAdapter? dataAdapter = dbProviderFactory.CreateDataAdapter();

            if (dataAdapter is null)
            {
                return null;
            }

            using DbDataSource dataSource =
                dbProviderFactory.CreateDataSource(connectionString);
            dataAdapter.SelectCommand = dataSource.CreateCommand(queryText);
            cancellationToken.ThrowIfCancellationRequested();
            dataAdapter.SelectCommand.CommandTimeout = commandTimeout;
            DataTable resultTable = new();
            dataAdapter.Fill(resultTable);
            return resultTable;
        }

        private async Task<DataTable?> QueryDataTableAsync(
            string connectionString,
            string queryText,
            int commandTimeout,
            CancellationToken cancellationToken)
        {
            await using DbDataSource dataSource =
                dbProviderFactory.CreateDataSource(connectionString);
            await using DbCommand queryCommand =
                dataSource.CreateCommand(queryText);
            cancellationToken.ThrowIfCancellationRequested();
            queryCommand.CommandTimeout = commandTimeout;
            await using DbDataReader reader =
                await queryCommand.ExecuteReaderAsync(cancellationToken);
            DataTable resultTable = new();
            resultTable.Load(reader);
            return resultTable;
        }
    }
}