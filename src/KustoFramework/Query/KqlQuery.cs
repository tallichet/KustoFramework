using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using KustoFramework.Attributes;

namespace KustoFramework.Query;

public class KqlQuery<T>
{
    private readonly string _tableName;
    private readonly ImmutableList<KqlClause> _clauses;

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

    public override string ToString() => ToKql();

    internal static string ResolveTableName()
    {
        var attr = typeof(T).GetCustomAttribute<KqlTableAttribute>();
        return attr?.Name ?? typeof(T).Name;
    }
}
