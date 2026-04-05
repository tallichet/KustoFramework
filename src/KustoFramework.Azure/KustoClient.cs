using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using KustoFramework.Azure.Materialization;
using KustoFramework.Query;

namespace KustoFramework.Azure;

/// <summary>
/// A Kusto client that extends <see cref="KustoContext"/> with query execution capabilities
/// against an Azure Data Explorer cluster.
/// </summary>
public sealed class KustoClient : KustoContext, IDisposable
{
    private readonly KustoConnectionOptions _options;
    private readonly ICslQueryProvider _queryProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="KustoClient"/> with the specified connection options.
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public KustoClient(KustoConnectionOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        var kcsb = options.BuildConnectionString();
        _queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);
    }

    /// <summary>
    /// The database name configured for this client.
    /// </summary>
    public string Database => _options.Database;

    /// <summary>
    /// Executes a KQL query and materializes the results into a list of <typeparamref name="T"/>.
    /// </summary>
    public async Task<List<T>> ExecuteQueryAsync<T>(KqlQuery<T> query, CancellationToken cancellationToken = default) where T : new()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var kql = query.ToKql();
        var crp = BuildRequestProperties();

        using var reader = await _queryProvider.ExecuteQueryAsync(
            _options.Database, kql, crp, cancellationToken).ConfigureAwait(false);

        return KqlResultMapper<T>.MapAll(reader);
    }

    /// <summary>
    /// Executes a KQL query and returns the first result, or default if no results are found.
    /// </summary>
    public async Task<T?> ExecuteFirstOrDefaultAsync<T>(KqlQuery<T> query, CancellationToken cancellationToken = default) where T : new()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var kql = query.ToKql();
        var crp = BuildRequestProperties();

        using var reader = await _queryProvider.ExecuteQueryAsync(
            _options.Database, kql, crp, cancellationToken).ConfigureAwait(false);

        if (!reader.Read())
            return default;

        var results = KqlResultMapper<T>.MapAll(reader);
        return results.Count > 0 ? results[0] : default;
    }

    /// <summary>
    /// Executes a KQL query and returns the raw <see cref="System.Data.IDataReader"/>
    /// for advanced scenarios. The caller is responsible for disposing the reader.
    /// </summary>
    public async Task<System.Data.IDataReader> ExecuteReaderAsync<T>(KqlQuery<T> query, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var kql = query.ToKql();
        var crp = BuildRequestProperties();

        return await _queryProvider.ExecuteQueryAsync(
            _options.Database, kql, crp, cancellationToken).ConfigureAwait(false);
    }

    private ClientRequestProperties BuildRequestProperties()
    {
        var crp = new ClientRequestProperties();
        crp.ClientRequestId = $"KustoFramework;{Guid.NewGuid():N}";

        if (_options.DefaultTimeout is { } timeout)
        {
            crp.SetOption(ClientRequestProperties.OptionServerTimeout, timeout.ToString());
        }

        return crp;
    }

    /// <summary>
    /// Disposes the client and its underlying resources. After disposal, the client should not be used for executing queries.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        (_queryProvider as IDisposable)?.Dispose();
    }
}
