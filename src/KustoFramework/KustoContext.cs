using KustoFramework.Attributes;
using KustoFramework.Query;
using System.Reflection;

namespace KustoFramework;

/// <summary>
/// Entry point for building KQL queries. Create a <see cref="KustoContext"/> instance and call
/// <see cref="Table{T}()"/> to start a query pipeline.
/// </summary>
/// <example>
/// <code>
/// var ctx = new KustoContext();
/// var kql = ctx.Table&lt;StormEvent&gt;().Where(e => e.State == "TEXAS").ToKql();
/// </code>
/// </example>
public class KustoContext
{
    /// <summary>Creates a query targeting the table name resolved from <typeparamref name="T"/> (using <see cref="KqlTableAttribute"/> or the type name).</summary>
    /// <typeparam name="T">The model type representing the table schema.</typeparam>
    public KqlQuery<T> Table<T>()
    {
        var tableName = KqlQuery<T>.ResolveTableName();
        return new KqlQuery<T>(tableName);
    }

    /// <summary>Creates a query targeting the specified <paramref name="tableName"/>.</summary>
    /// <typeparam name="T">The model type representing the table schema.</typeparam>
    /// <param name="tableName">The explicit KQL table name.</param>
    public KqlQuery<T> Table<T>(string tableName) =>
        new KqlQuery<T>(tableName);
}
