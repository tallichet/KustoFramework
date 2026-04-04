using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using KustoFramework.Attributes;

namespace KustoFramework.Query;

/// <summary>
/// An immutable KQL query pipeline. Each operator method returns a new instance with the clause appended.
/// Call <see cref="ToKql"/> to render the final KQL string.
/// </summary>
/// <typeparam name="T">The model type representing the current output schema.</typeparam>
public class KqlQuery<T>
{
    private readonly string _tableName;
    private readonly ImmutableList<KqlClause> _clauses;

    /// <summary>Creates a new query for the specified table.</summary>
    /// <param name="tableName">The KQL table name.</param>
    public KqlQuery(string tableName) : this(tableName, ImmutableList<KqlClause>.Empty) { }

    internal KqlQuery(string tableName, ImmutableList<KqlClause> clauses)
    {
        _tableName = tableName;
        _clauses = clauses;
    }

    internal KqlQuery<TResult> WithClause<TResult>(KqlClause clause) =>
        new(_tableName, _clauses.Add(clause));

    internal KqlQuery<T> WithClause(KqlClause clause) =>
        new(_tableName, _clauses.Add(clause));

    internal KqlOrderedQuery<T> WithOrderClause(OrderByClause clause) =>
        new(_tableName, _clauses.Add(clause));

    internal string GetTableName() => _tableName;

    internal ImmutableList<KqlClause> GetClauses() => _clauses;

    /// <summary>Renders the complete KQL query string, including the table name and all appended operators.</summary>
    /// <returns>The KQL query as a string.</returns>
    public string ToKql()
    {
        var visitor = new KqlExpressionVisitor();
        var sb = new StringBuilder();
        sb.Append(_tableName);

        foreach (var clause in _clauses)
        {
            sb.Append('\n');
            sb.Append(clause.ToKql(visitor));
        }

        return sb.ToString();
    }

    /// <summary>Returns the KQL string representation of this query (same as <see cref="ToKql"/>).</summary>
    public override string ToString() => ToKql();

    internal static string ResolveTableName()
    {
        var attr = typeof(T).GetCustomAttribute<KqlTableAttribute>();
        return attr?.Name ?? typeof(T).Name;
    }
}
