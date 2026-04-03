using Microsoft.Extensions.DependencyInjection;

namespace KustoFramework.Azure.DependencyInjection;

/// <summary>
/// Extension methods for registering <see cref="KustoClient"/> in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="KustoClient"/> as a singleton in the service collection.
    /// </summary>
    /// <example>
    /// <code>
    /// services.AddKustoClient(options =>
    /// {
    ///     options.ClusterUri = "https://mycluster.kusto.windows.net";
    ///     options.Database = "MyDatabase";
    ///     options.ConfigureConnection = kcsb => kcsb.WithAadManagedIdentity("system");
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddKustoClient(this IServiceCollection services, Action<KustoConnectionOptions> configure)
    {
        var options = new KustoConnectionOptions();
        configure(options);

        var client = new KustoClient(options);
        services.AddSingleton(client);
        services.AddSingleton<KustoContext>(client);

        return services;
    }
}
