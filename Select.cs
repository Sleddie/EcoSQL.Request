using System.Text;

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
    /// <param name="getCount">The function for getting the count of items.</param>
    /// <param name="queryAdditionalInfo">The additional information for the query to display.</param>
    /// <returns>True if data was obtained successfully (not null); otherwise, false.</returns>
    private static bool SelectData<TData>(
        QueryData<TData> queryData,
        string connectionString,
        string queryText,
        OperationUnit queryInfoUnit,
        int commandTimeout,
        CancellationToken cancellationToken,
        out TData? obtainedData,
        Func<TData, int>? getCount = null,
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
    private static bool SelectData<TData>(
        QueryData<TData> queryData,
        string connectionString,
        string queryText,
        ResultUnit<TData> obtainedDataInfoUnit,
        int commandTimeout,
        CancellationToken cancellationToken,
        Func<TData, int>? getCount = null,
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
    private static async Task<bool> SelectDataAsync<TData>(
        QueryDataAsync<TData> queryDataAsync,
        string connectionString,
        string queryText,
        ResultUnit<TData> obtainedDataInfoUnit,
        int commandTimeout,
        CancellationToken cancellationToken,
        Func<TData, int>? getCount = null,
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

    /// <summary>Gets the full message of an exception, including inner exceptions.</summary>
    /// <param name="ex">The exception for which to get the full message.</param>
    /// <returns>The full message of the exception.</returns>
    private static string GetExceptionFullMessage(this Exception ex)
    {
        if (ex.InnerException is null)
        {
            return ex.Message;
        }

        StringBuilder fullMessageBuilder = new(ex.Message);
        Exception? innerEx = ex.InnerException;

        do
        {
            fullMessageBuilder.Append($"{Environment.NewLine}\t{ex.Message}");
        } while ((innerEx = innerEx?.InnerException) is not null);

        return fullMessageBuilder.ToString();
    }

    private static void FinalizeOperationInfo(
        OperationUnit operationInfoUnit,
        string statusMessage,
        string? queryAdditionalInfo = null)
    {
        bool hasAdditionalInfo = !string.IsNullOrEmpty(queryAdditionalInfo);

        if (hasAdditionalInfo)
        {
            operationInfoUnit.Message = $"Database query info: {queryAdditionalInfo}";
        }

        operationInfoUnit.Message = $"Query status: {statusMessage}";
    }

    private static void FinalizeOperationInfo(
        OperationUnit operationInfoUnit,
        string statusMessage,
        int itemsCount,
        string? queryAdditionalInfo = null)
    {
        FinalizeOperationInfo(operationInfoUnit, statusMessage, queryAdditionalInfo: queryAdditionalInfo);
        operationInfoUnit.Message = $"Obtained items count: {itemsCount}";
    }

    /// <summary>Represents a delegate for querying data.</summary>
    /// <typeparam name="TData">The type of the data to select.</typeparam>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="queryText">The text of the query.</param>
    /// <param name="commandTimeout">The query command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private delegate TData? QueryData<TData>(
        string connectionString,
        string queryText,
        int commandTimeout,
        CancellationToken cancellationToken)
        where TData : class;

    /// <summary>Represents a delegate for querying data asynchronously.</summary>
    /// <typeparam name="TData">The type of the data to select.</typeparam>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="queryText">The text of the query.</param>
    /// <param name="commandTimeout">The query command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private delegate Task<TData?> QueryDataAsync<TData>(
        string connectionString,
        string queryText,
        int commandTimeout,
        CancellationToken cancellationToken)
        where TData : class;
}