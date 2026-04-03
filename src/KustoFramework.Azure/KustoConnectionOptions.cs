using Kusto.Data;

namespace KustoFramework.Azure;

/// <summary>
/// Configuration options for connecting to an Azure Data Explorer cluster.
/// </summary>
public sealed class KustoConnectionOptions
{
    /// <summary>
    /// The Azure Data Explorer cluster URI (e.g. "https://mycluster.kusto.windows.net").
    /// </summary>
    public string ClusterUri { get; set; } = "";

    /// <summary>
    /// The default database to query against.
    /// </summary>
    public string Database { get; set; } = "";

    /// <summary>
    /// Default server timeout for queries. If null, the ADX default is used.
    /// </summary>
    public TimeSpan? DefaultTimeout { get; set; }

    /// <summary>
    /// Optional callback to fully configure the <see cref="KustoConnectionStringBuilder"/>.
    /// This is called after the <see cref="ClusterUri"/> is set, giving full control over
    /// authentication and advanced connection properties.
    /// </summary>
    /// <example>
    /// <code>
    /// options.ConfigureConnection = kcsb => kcsb.WithAadApplicationKeyAuthentication(appId, appKey, authority);
    /// </code>
    /// </example>
    public Action<KustoConnectionStringBuilder>? ConfigureConnection { get; set; }

    internal KustoConnectionStringBuilder BuildConnectionString()
    {
        if (string.IsNullOrWhiteSpace(ClusterUri))
            throw new InvalidOperationException($"{nameof(ClusterUri)} must be configured.");

        if (string.IsNullOrWhiteSpace(Database))
            throw new InvalidOperationException($"{nameof(Database)} must be configured.");

        var kcsb = new KustoConnectionStringBuilder(ClusterUri);
        ConfigureConnection?.Invoke(kcsb);
        return kcsb;
    }
}
