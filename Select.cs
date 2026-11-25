namespace EcoSQL.Request;

/// <summary>Provides static and extension methods for querying data from a database.</summary>
public static partial class Select
{
    /// <summary>Selects data from the database.</summary>
    /// <typeparam name="TData">The type of the data to select.</typeparam>
    /// <param name="queryData">The delegate for querying data.</param>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="queryText">The text of the query.</param>
    /// <param name="queryInfoUnit">The query progress information unit.</param>
    /// <param name="commandTimeout">The query command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="obtainedData">The obtained data.</param>
    /// <param name="queryAdditionalInfo">The additional information for the query to display.</param>
    /// <returns>True if data was obtained successfully (not null); otherwise, false.</returns>
    public static bool SelectData<TData>(
        QueryData<TData> queryData,
        string connectionString,
        string queryText,
        OperationUnit queryInfoUnit,
        int commandTimeout,
        CancellationToken cancellationToken,
        out TData? obtainedData,
        string? queryAdditionalInfo = null)
        where TData : class
    {
        string statusMessage = "";
        obtainedData = null;

        try
        {
            obtainedData = queryData(
                connectionString,
                queryText,
                commandTimeout,
                cancellationToken);
            statusMessage = "Success";
            return obtainedData is not null;
        }
        catch (OperationCanceledException)
        {
            statusMessage = "Cancelled";
            obtainedData = null;
            return false;
        }
        catch (Exception ex)
        {
            statusMessage = $"Error:{Environment.NewLine}" +
                $"\t{ex?.GetExceptionFullMessage()}";
            obtainedData = null;
            return false;
        }
        finally
        {
            FinalizeOperationInfo(
                queryInfoUnit,
                statusMessage,
                queryAdditionalInfo: queryAdditionalInfo);
        }
    }

    /// <summary>Selects data from the database.</summary>
    /// <typeparam name="TData">The type of the data to select.</typeparam>
    /// <param name="queryData">The delegate for querying data.</param>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="queryText">The text of the query.</param>
    /// <param name="obtainedDataInfoUnit">The query progress information and obtained data unit.</param>
    /// <param name="commandTimeout">The query command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="queryAdditionalInfo">The additional information for the query to display.</param>
    /// <returns>True if data was obtained successfully (not null); otherwise, false.</returns>
    public static bool SelectData<TData>(
        QueryData<TData> queryData,
        string connectionString,
        string queryText,
        ResultUnit<TData> obtainedDataInfoUnit,
        int commandTimeout,
        CancellationToken cancellationToken,
        string? queryAdditionalInfo = null)
        where TData : class
    {
        bool isObtained = SelectData(
            queryData,
            connectionString,
            queryText,
            obtainedDataInfoUnit,
            commandTimeout,
            cancellationToken,
            out TData? obtainedData,
            queryAdditionalInfo: queryAdditionalInfo);
        obtainedDataInfoUnit.Result = obtainedData;
        return isObtained;
    }

    /// <summary>Selects data from the database asynchronously.</summary>
    /// <typeparam name="TData">The type of the data to select.</typeparam>
    /// <param name="queryDataAsync">The delegate for querying data asynchronously.</param>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="queryText">The text of the query.</param>
    /// <param name="obtainedDataInfoUnit">The query progress information and obtained data unit.</param>
    /// <param name="commandTimeout">The query command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="queryAdditionalInfo">The additional information for the query to display.</param>
    /// <returns>True if data was obtained successfully (not null); otherwise, false.</returns>
    public static async Task<bool> SelectDataAsync<TData>(
        QueryDataAsync<TData> queryDataAsync,
        string connectionString,
        string queryText,
        ResultUnit<TData> obtainedDataInfoUnit,
        int commandTimeout,
        CancellationToken cancellationToken,
        string? queryAdditionalInfo = null)
        where TData : class
    {
        string statusMessage = "";
        TData? obtainedData = null;

        try
        {
            obtainedData = await queryDataAsync(
                connectionString,
                queryText,
                commandTimeout,
                cancellationToken);
            statusMessage = "Success";
            return obtainedData is not null;
        }
        catch (OperationCanceledException)
        {
            statusMessage = "Cancelled";
            obtainedData = null;
            return false;
        }
        catch (Exception ex)
        {
            statusMessage = $"Error:{Environment.NewLine}" +
                $"\t{ex?.GetExceptionFullMessage()}";
            obtainedData = null;
            return false;
        }
        finally
        {
            FinalizeOperationInfo(
                obtainedDataInfoUnit,
                statusMessage,
                queryAdditionalInfo: queryAdditionalInfo);
            obtainedDataInfoUnit.Result = obtainedData;
        }
    }

    private static void FinalizeOperationInfo(
        OperationUnit operationInfoUnit,
        string statusMessage,
        string? queryAdditionalInfo = null)
    {
        string fullMessage = $"Query status: {statusMessage}";
        bool hasAdditionalInfo = !string.IsNullOrEmpty(queryAdditionalInfo);

        if (hasAdditionalInfo)
        {
            fullMessage = $"Database query info: {queryAdditionalInfo}{Environment.NewLine}\t" +
                fullMessage;
        }

        operationInfoUnit.Message = fullMessage;
    }

    #region obsolete
        
    /// <summary>Selects data from the database.</summary>
    /// <typeparam name="TData">The type of the data to select.</typeparam>
    /// <param name="queryData">The delegate for querying data.</param>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="queryText">The text of the query.</param>
    /// <param name="queryInfoUnit">The query progress information unit.</param>
    /// <param name="commandTimeout">The query command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="obtainedData">The obtained data.</param>
    /// <param name="getCount">The function for getting the count of items.</param>
    /// <param name="queryAdditionalInfo">The additional information for the query to display.</param>
    /// <returns>True if data was obtained successfully (not null); otherwise, false.</returns>
    [Obsolete("The number of obtained items is no longer supported.")]
    public static bool SelectData<TData>(
        QueryData<TData> queryData,
        string connectionString,
        string queryText,
        OperationUnit queryInfoUnit,
        int commandTimeout,
        CancellationToken cancellationToken,
        out TData? obtainedData,
        Func<TData, int>? getCount,
        string? queryAdditionalInfo = null)
        where TData : class
    {
        string statusMessage = "";
        obtainedData = null;

        try
        {
            obtainedData = queryData(
                connectionString,
                queryText,
                commandTimeout,
                cancellationToken);
            statusMessage = "Success";
            return obtainedData is not null;
        }
        catch (OperationCanceledException)
        {
            statusMessage = "Cancelled";
            obtainedData = null;
            return false;
        }
        catch (Exception ex)
        {
            statusMessage = $"Error:{Environment.NewLine}" +
                $"\t{ex?.GetExceptionFullMessage()}";
            obtainedData = null;
            return false;
        }
        finally
        {
            if (getCount is null || obtainedData is null)
            {
                FinalizeOperationInfo(
                    queryInfoUnit,
                    statusMessage,
                    queryAdditionalInfo: queryAdditionalInfo);
            }
            else
            {
                int itemsCount = getCount(obtainedData);
                FinalizeOperationInfo(
                    queryInfoUnit,
                    statusMessage,
                    itemsCount,
                    queryAdditionalInfo: queryAdditionalInfo);
            }
        }
    }

    /// <summary>Selects data from the database.</summary>
    /// <typeparam name="TData">The type of the data to select.</typeparam>
    /// <param name="queryData">The delegate for querying data.</param>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="queryText">The text of the query.</param>
    /// <param name="obtainedDataInfoUnit">The query progress information and obtained data unit.</param>
    /// <param name="commandTimeout">The query command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="getCount">The function for getting the count of items.</param>
    /// <param name="queryAdditionalInfo">The additional information for the query to display.</param>
    /// <returns>True if data was obtained successfully (not null); otherwise, false.</returns>
    [Obsolete("The number of obtained items is no longer supported.")]
    public static bool SelectData<TData>(
        QueryData<TData> queryData,
        string connectionString,
        string queryText,
        ResultUnit<TData> obtainedDataInfoUnit,
        int commandTimeout,
        CancellationToken cancellationToken,
        Func<TData, int>? getCount,
        string? queryAdditionalInfo = null)
        where TData : class
    {
        bool isObtained = SelectData(
            queryData,
            connectionString,
            queryText,
            obtainedDataInfoUnit,
            commandTimeout,
            cancellationToken,
            out TData? obtainedData,
            getCount: getCount,
            queryAdditionalInfo: queryAdditionalInfo);
        obtainedDataInfoUnit.Result = obtainedData;
        return isObtained;
    }

    /// <summary>Selects data from the database asynchronously.</summary>
    /// <typeparam name="TData">The type of the data to select.</typeparam>
    /// <param name="queryDataAsync">The delegate for querying data asynchronously.</param>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="queryText">The text of the query.</param>
    /// <param name="obtainedDataInfoUnit">The query progress information and obtained data unit.</param>
    /// <param name="commandTimeout">The query command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="getCount">The function for getting the count of items.</param>
    /// <param name="queryAdditionalInfo">The additional information for the query to display.</param>
    /// <returns>True if data was obtained successfully (not null); otherwise, false.</returns>
    [Obsolete("The number of obtained items is no longer supported.")]
    public static async Task<bool> SelectDataAsync<TData>(
        QueryDataAsync<TData> queryDataAsync,
        string connectionString,
        string queryText,
        ResultUnit<TData> obtainedDataInfoUnit,
        int commandTimeout,
        CancellationToken cancellationToken,
        Func<TData, int>? getCount,
        string? queryAdditionalInfo = null)
        where TData : class
    {
        string statusMessage = "";
        TData? obtainedData = null;

        try
        {
            obtainedData = await queryDataAsync(
                connectionString,
                queryText,
                commandTimeout,
                cancellationToken);
            statusMessage = "Success";
            return obtainedData is not null;
        }
        catch (OperationCanceledException)
        {
            statusMessage = "Cancelled";
            obtainedData = null;
            return false;
        }
        catch (Exception ex)
        {
            statusMessage = $"Error:{Environment.NewLine}" +
                $"\t{ex?.GetExceptionFullMessage()}";
            obtainedData = null;
            return false;
        }
        finally
        {
            if (getCount is null || obtainedData is null)
            {
                FinalizeOperationInfo(
                    obtainedDataInfoUnit,
                    statusMessage,
                    queryAdditionalInfo: queryAdditionalInfo);
            }
            else
            {
                int itemsCount = getCount(obtainedData);
                FinalizeOperationInfo(
                    obtainedDataInfoUnit,
                    statusMessage,
                    itemsCount,
                    queryAdditionalInfo: queryAdditionalInfo);
            }

            obtainedDataInfoUnit.Result = obtainedData;
        }
    }

    [Obsolete("The number of obtained items is no longer supported.")]
    private static void FinalizeOperationInfo(
        OperationUnit operationInfoUnit,
        string statusMessage,
        int itemsCount,
        string? queryAdditionalInfo = null)
    {
        FinalizeOperationInfo(operationInfoUnit, statusMessage, queryAdditionalInfo: queryAdditionalInfo);
        operationInfoUnit.Message = $"Obtained items count: {itemsCount}";
    }

    #endregion
}
