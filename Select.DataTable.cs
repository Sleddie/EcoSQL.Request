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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="commandTimeout">Query command timeout</param>
        /// <param name="resultTableInfoUnit">Query execution information unit</param>
        /// <param name="queryAdditionalInfo">Additional query information</param>
        /// <returns>True if the data table was obtained successfully</returns>
        public bool SelectDataTable(
            string connectionString,
            string queryText,
            CancellationToken cancellationToken,
            int commandTimeout,
            ResultTableUnit resultTableInfoUnit,
            string? queryAdditionalInfo = null)
        {
            return dbProviderFactory.SelectData(
                connectionString,
                queryText,
                cancellationToken,
                commandTimeout,
                dbProviderFactory.QueryDataTable,
                resultTableInfoUnit,
                getCount: dataTable => dataTable.Rows.Count,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        /// <summary>Asynchronously selects a data table from the database</summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="queryText">SQL query text</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="commandTimeout">Query command timeout</param>
        /// <param name="resultTableInfoUnit">Query execution information unit</param>
        /// <param name="queryAdditionalInfo">Additional query information</param>
        /// <returns>True if the data table was obtained successfully</returns>
        public async Task<bool> SelectDataTableAsync(
            string connectionString,
            string queryText,
            CancellationToken cancellationToken,
            int commandTimeout,
            ResultTableUnit resultTableInfoUnit,
            string? queryAdditionalInfo = null)
        {
            return await dbProviderFactory.SelectDataAsync(
                connectionString,
                queryText,
                cancellationToken,
                commandTimeout,
                dbProviderFactory.QueryDataTableAsync,
                resultTableInfoUnit,
                getCount: dataTable => dataTable.Rows.Count,
                queryAdditionalInfo: queryAdditionalInfo);
        }

        private DataTable? QueryDataTable(
            string connectionString,
            string queryText,
            CancellationToken cancellationToken,
            int commandTimeout)
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
            CancellationToken cancellationToken,
            int commandTimeout)
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