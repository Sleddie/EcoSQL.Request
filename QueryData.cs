namespace EcoSQL.Request;

/// <summary>Represents a delegate for querying data.</summary>
/// <typeparam name="TData">The type of the data to select.</typeparam>
/// <param name="connectionString">The connection string for the database.</param>
/// <param name="queryText">The text of the query.</param>
/// <param name="commandTimeout">The query command timeout.</param>
/// <param name="cancellationToken">The cancellation token.</param>
public delegate TData? QueryData<TData>(
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
public delegate Task<TData?> QueryDataAsync<TData>(
    string connectionString,
    string queryText,
    int commandTimeout,
    CancellationToken cancellationToken)
    where TData : class;