using System.Linq.Expressions;
using KustoFramework.Enums;
using KustoFramework.Query;

namespace KustoFramework.Extensions;

public static class KqlQueryExtensions
{
    // Where
    public static KqlQuery<T> Where<T>(this KqlQuery<T> source, Expression<Func<T, bool>> predicate) =>
        source.WithClause(new WhereClause(predicate));

    // Project
    public static KqlQuery<TResult> Project<T, TResult>(this KqlQuery<T> source, Expression<Func<T, TResult>> selector) =>
        source.WithClause<TResult>(new ProjectClause(selector));

    // ProjectAway
    public static KqlQuery<T> ProjectAway<T, TCol>(this KqlQuery<T> source, params Expression<Func<T, TCol>>[] columns) =>
        source.WithClause(new ProjectAwayClause(columns.Cast<LambdaExpression>().ToArray()));

    // Extend
    public static KqlQuery<TResult> Extend<T, TResult>(this KqlQuery<T> source, Expression<Func<T, TResult>> extension) =>
        source.WithClause<TResult>(new ExtendClause(extension));

    // OrderBy
    public static KqlOrderedQuery<T> OrderBy<T, TKey>(this KqlQuery<T> source, Expression<Func<T, TKey>> keySelector) =>
        source.WithOrderClause(new OrderByClause([(keySelector, SortOrder.Ascending)]));

    public static KqlOrderedQuery<T> OrderByDescending<T, TKey>(this KqlQuery<T> source, Expression<Func<T, TKey>> keySelector) =>
        source.WithOrderClause(new OrderByClause([(keySelector, SortOrder.Descending)]));

    // ThenBy
    public static KqlOrderedQuery<T> ThenBy<T, TKey>(this KqlOrderedQuery<T> source, Expression<Func<T, TKey>> keySelector) =>
        source.AddThenBy(keySelector, SortOrder.Ascending);

    public static KqlOrderedQuery<T> ThenByDescending<T, TKey>(this KqlOrderedQuery<T> source, Expression<Func<T, TKey>> keySelector) =>
        source.AddThenBy(keySelector, SortOrder.Descending);

    // Top
    public static KqlQuery<T> Top<T, TKey>(this KqlQuery<T> source, int count, Expression<Func<T, TKey>> orderBy, SortOrder order = SortOrder.Descending) =>
        source.WithClause(new TopClause(count, orderBy, order));

    // Take
    public static KqlQuery<T> Take<T>(this KqlQuery<T> source, int count) =>
        source.WithClause(new TakeClause(count));

    // Distinct
    public static KqlQuery<T> Distinct<T>(this KqlQuery<T> source) =>
        source.WithClause(new DistinctClause(null));

    public static KqlQuery<TResult> Distinct<T, TResult>(this KqlQuery<T> source, Expression<Func<T, TResult>> selector) =>
        source.WithClause<TResult>(new DistinctClause(selector));

    // Count
    public static KqlQuery<T> Count<T>(this KqlQuery<T> source) =>
        source.WithClause(new CountClause());

    // Summarize
    public static KqlQuery<TResult> Summarize<T, TResult>(this KqlQuery<T> source, Expression<Func<T, TResult>> aggregation) =>
        source.WithClause<TResult>(new SummarizeClause(aggregation, null));

    public static KqlQuery<TResult> Summarize<T, TKey, TResult>(
        this KqlQuery<T> source,
        Expression<Func<T, TKey>> groupBy,
        Expression<Func<T, TResult>> aggregation) =>
        source.WithClause<TResult>(new SummarizeClause(aggregation, groupBy));

    // Join
    public static KqlQuery<TResult> Join<TOuter, TInner, TKey, TResult>(
        this KqlQuery<TOuter> outer,
        KqlQuery<TInner> inner,
        Expression<Func<TOuter, TKey>> outerKey,
        Expression<Func<TInner, TKey>> innerKey,
        Expression<Func<TOuter, TInner, TResult>> resultSelector,
        JoinKind kind = JoinKind.InnerUnique)
    {
        var innerKql = inner.ToKql();
        var query = outer.WithClause<TResult>(new JoinClause(innerKql, outerKey, innerKey, kind));
        if (resultSelector != null)
            return query.WithClause<TResult>(new ProjectClause(resultSelector));
        return query;
    }

    // Union
    public static KqlQuery<T> Union<T>(this KqlQuery<T> source, params KqlQuery<T>[] others)
    {
        var otherKqls = others.Select(o => o.ToKql()).ToArray();
        return source.WithClause(new UnionClause(otherKqls, null));
    }

    public static KqlQuery<T> Union<T>(this KqlQuery<T> source, UnionKind kind, params KqlQuery<T>[] others)
    {
        var otherKqls = others.Select(o => o.ToKql()).ToArray();
        return source.WithClause(new UnionClause(otherKqls, kind));
    }

    // MvExpand
    public static KqlQuery<T> MvExpand<T>(this KqlQuery<T> source, Expression<Func<T, object>> arrayColumn) =>
        source.WithClause(new MvExpandClause(arrayColumn));

    // Parse
    public static KqlQuery<TResult> Parse<T, TResult>(this KqlQuery<T> source, Expression<Func<T, string>> column, string pattern) =>
        source.WithClause<TResult>(new ParseClause(column, pattern));

    // BagUnpack
    public static KqlQuery<TResult> BagUnpack<T, TResult>(this KqlQuery<T> source, Expression<Func<T, object>> column) =>
        source.WithClause<TResult>(new BagUnpackClause(column));

    // Render
    public static KqlQuery<T> Render<T>(this KqlQuery<T> source, RenderKind kind) =>
        source.WithClause(new RenderClause(kind));
}
