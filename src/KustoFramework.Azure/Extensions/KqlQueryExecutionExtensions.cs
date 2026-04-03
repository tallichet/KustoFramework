using KustoFramework.Query;

namespace KustoFramework.Azure.Extensions;

/// <summary>
/// Extension methods on <see cref="KqlQuery{T}"/> for executing queries against Azure Data Explorer.
/// </summary>
public static class KqlQueryExecutionExtensions
{
    /// <summary>
    /// Executes the query and materializes all results into a list.
    /// </summary>
    public static Task<List<T>> ToListAsync<T>(this KqlQuery<T> query, KustoClient client, CancellationToken cancellationToken = default) where T : new()
        => client.ExecuteQueryAsync(query, cancellationToken);

    /// <summary>
    /// Executes the query and returns the first result, or default if no results are found.
    /// </summary>
    public static Task<T?> FirstOrDefaultAsync<T>(this KqlQuery<T> query, KustoClient client, CancellationToken cancellationToken = default) where T : new()
        => client.ExecuteFirstOrDefaultAsync(query, cancellationToken);

    /// <summary>
    /// Executes the query and returns the raw <see cref="System.Data.IDataReader"/>.
    /// The caller is responsible for disposing the reader.
    /// </summary>
    public static Task<System.Data.IDataReader> ToDataReaderAsync<T>(this KqlQuery<T> query, KustoClient client, CancellationToken cancellationToken = default)
        => client.ExecuteReaderAsync(query, cancellationToken);
}
