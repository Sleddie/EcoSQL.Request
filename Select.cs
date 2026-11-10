using System.Data.Common;
using System.Text;

namespace EcoSQL.Request;

public static partial class Select
{
    extension(DbProviderFactory dbProviderFactory)
    {
        private bool SelectData<TData>(
            string connectionString,
            string queryText,
            CancellationToken cancellationToken,
            int commandTimeout,
            QueryData<TData> queryData,
            OperationUnit queryInfoUnit,
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
                    cancellationToken,
                    commandTimeout);
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

        private bool SelectData<TData>(
            string connectionString,
            string queryText,
            CancellationToken cancellationToken,
            int commandTimeout,
            QueryData<TData> queryData,
            ResultUnit<TData> obtainedDataInfoUnit,
            Func<TData, int>? getCount = null,
            string? queryAdditionalInfo = null)
            where TData : class
        {
            bool isObtained = dbProviderFactory.SelectData(
                connectionString,
                queryText,
                cancellationToken,
                commandTimeout,
                queryData,
                obtainedDataInfoUnit,
                out TData? obtainedData,
                getCount: getCount,
                queryAdditionalInfo: queryAdditionalInfo);
            obtainedDataInfoUnit.Result = obtainedData;
            return isObtained;
        }

        private async Task<bool> SelectDataAsync<TData>(
            string connectionString,
            string queryText,
            CancellationToken cancellationToken,
            int commandTimeout,
            QueryDataAsync<TData> queryDataAsync,
            ResultUnit<TData> obtainedDataInfoUnit,
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
                    cancellationToken,
                    commandTimeout);
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
    }

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
        operationInfoUnit.Count++;
        bool hasAdditionalInfo = !string.IsNullOrEmpty(queryAdditionalInfo);

        if (hasAdditionalInfo)
        {
            operationInfoUnit.Message = $"Database query: {queryAdditionalInfo}";
        }

        operationInfoUnit.Message =
            hasAdditionalInfo ? "" : "Query №" +
            $"{operationInfoUnit.Count} out of {operationInfoUnit.TotalCount}: {statusMessage}";
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

    private delegate TData? QueryData<TData>(
        string connectionString,
        string queryText,
        CancellationToken cancellationToken,
        int commandTimeout)
        where TData : class;

    private delegate Task<TData?> QueryDataAsync<TData>(
        string connectionString,
        string queryText,
        CancellationToken cancellationToken,
        int commandTimeout)
        where TData : class;
}