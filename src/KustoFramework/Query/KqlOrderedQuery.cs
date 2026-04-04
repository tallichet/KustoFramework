using System.Collections.Immutable;
using KustoFramework.Enums;

namespace KustoFramework.Query;

/// <summary>
/// Represents an ordered KQL query, enabling secondary sort keys via <c>ThenBy</c> / <c>ThenByDescending</c>.
/// </summary>
/// <typeparam name="T">The model type representing the current output schema.</typeparam>
public class KqlOrderedQuery<T> : KqlQuery<T>
{
    internal KqlOrderedQuery(string tableName, ImmutableList<KqlClause> clauses)
        : base(tableName, clauses) { }

    internal OrderByClause GetLastOrderByClause()
    {
        for (int i = GetClauses().Count - 1; i >= 0; i--)
        {
            if (GetClauses()[i] is OrderByClause obc)
                return obc;
        }
        throw new InvalidOperationException("No OrderBy clause found.");
    }

    internal KqlOrderedQuery<T> AddThenBy(System.Linq.Expressions.LambdaExpression keySelector, SortOrder order)
    {
        var clauses = GetClauses();
        var lastOrderBy = GetLastOrderByClause();
        var newKeys = new List<(System.Linq.Expressions.LambdaExpression, SortOrder)>(lastOrderBy.Keys)
        {
            (keySelector, order)
        };
        var newClause = new OrderByClause(newKeys);

        // Replace the last OrderByClause
        var builder = clauses.ToBuilder();
        for (int i = builder.Count - 1; i >= 0; i--)
        {
            if (builder[i] is OrderByClause)
            {
                builder[i] = newClause;
                break;
            }
        }

        return new KqlOrderedQuery<T>(GetTableName(), builder.ToImmutable());
    }
}
