using System.Linq.Expressions;
using KustoFramework.Enums;
using KustoFramework.Query;

namespace KustoFramework.Extensions;

/// <summary>
/// Extension methods for building KQL queries with a LINQ-like fluent API.
/// Each method appends a KQL tabular operator to the query pipeline.
/// </summary>
public static class KqlQueryExtensions
{
    /// <summary>Filters records by a boolean <paramref name="predicate"/>. KQL: <c>| where predicate</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="predicate">A boolean expression to filter records.</param>
    public static KqlQuery<T> Where<T>(this KqlQuery<T> source, Expression<Func<T, bool>> predicate) =>
        source.WithClause(new WhereClause(predicate));

    /// <summary>Selects a subset of columns or computes new ones. KQL: <c>| project columns</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="selector">An expression selecting the output columns.</param>
    public static KqlQuery<TResult> Project<T, TResult>(this KqlQuery<T> source, Expression<Func<T, TResult>> selector) =>
        source.WithClause<TResult>(new ProjectClause(selector));

    /// <summary>Removes specified columns from the output. KQL: <c>| project-away columns</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="columns">Expressions identifying the columns to remove.</param>
    public static KqlQuery<T> ProjectAway<T, TCol>(this KqlQuery<T> source, params Expression<Func<T, TCol>>[] columns) =>
        source.WithClause(new ProjectAwayClause(columns.Cast<LambdaExpression>().ToArray()));

    /// <summary>Adds computed columns to the output while keeping all existing columns. KQL: <c>| extend columns</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="extension">An expression defining the new columns.</param>
    public static KqlQuery<TResult> Extend<T, TResult>(this KqlQuery<T> source, Expression<Func<T, TResult>> extension) =>
        source.WithClause<TResult>(new ExtendClause(extension));

    /// <summary>Sorts the results in ascending order by the specified key. KQL: <c>| sort by column asc</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="keySelector">The column to sort by.</param>
    public static KqlOrderedQuery<T> OrderBy<T, TKey>(this KqlQuery<T> source, Expression<Func<T, TKey>> keySelector) =>
        source.WithOrderClause(new OrderByClause([(keySelector, SortOrder.Ascending)]));

    /// <summary>Sorts the results in descending order by the specified key. KQL: <c>| sort by column desc</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="keySelector">The column to sort by.</param>
    public static KqlOrderedQuery<T> OrderByDescending<T, TKey>(this KqlQuery<T> source, Expression<Func<T, TKey>> keySelector) =>
        source.WithOrderClause(new OrderByClause([(keySelector, SortOrder.Descending)]));

    /// <summary>Adds a secondary ascending sort key. KQL: appended to <c>sort by ... , column asc</c>.</summary>
    /// <param name="source">The ordered query.</param>
    /// <param name="keySelector">The secondary sort column.</param>
    public static KqlOrderedQuery<T> ThenBy<T, TKey>(this KqlOrderedQuery<T> source, Expression<Func<T, TKey>> keySelector) =>
        source.AddThenBy(keySelector, SortOrder.Ascending);

    /// <summary>Adds a secondary descending sort key. KQL: appended to <c>sort by ... , column desc</c>.</summary>
    /// <param name="source">The ordered query.</param>
    /// <param name="keySelector">The secondary sort column.</param>
    public static KqlOrderedQuery<T> ThenByDescending<T, TKey>(this KqlOrderedQuery<T> source, Expression<Func<T, TKey>> keySelector) =>
        source.AddThenBy(keySelector, SortOrder.Descending);

    /// <summary>Returns the top N records sorted by the specified column. KQL: <c>| top N by column</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="count">The number of records to return.</param>
    /// <param name="orderBy">The column to sort by.</param>
    /// <param name="order">The sort direction (default: <see cref="SortOrder.Descending"/>).</param>
    public static KqlQuery<T> Top<T, TKey>(this KqlQuery<T> source, int count, Expression<Func<T, TKey>> orderBy, SortOrder order = SortOrder.Descending) =>
        source.WithClause(new TopClause(count, orderBy, order));

    /// <summary>Returns the first <paramref name="count"/> records. KQL: <c>| take N</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="count">The number of records to take.</param>
    public static KqlQuery<T> Take<T>(this KqlQuery<T> source, int count) =>
        source.WithClause(new TakeClause(count));

    /// <summary>Returns distinct rows across all columns. KQL: <c>| distinct</c>.</summary>
    /// <param name="source">The source query.</param>
    public static KqlQuery<T> Distinct<T>(this KqlQuery<T> source) =>
        source.WithClause(new DistinctClause(null));

    /// <summary>Returns distinct rows for the selected columns. KQL: <c>| distinct columns</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="selector">An expression selecting the columns for distinct evaluation.</param>
    public static KqlQuery<TResult> Distinct<T, TResult>(this KqlQuery<T> source, Expression<Func<T, TResult>> selector) =>
        source.WithClause<TResult>(new DistinctClause(selector));

    /// <summary>Counts the number of records. KQL: <c>| count</c>.</summary>
    /// <param name="source">The source query.</param>
    public static KqlQuery<T> Count<T>(this KqlQuery<T> source) =>
        source.WithClause(new CountClause());

    /// <summary>Produces aggregated values with no group-by key. KQL: <c>| summarize aggregation</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="aggregation">An expression defining the aggregation functions.</param>
    public static KqlQuery<TResult> Summarize<T, TResult>(this KqlQuery<T> source, Expression<Func<T, TResult>> aggregation) =>
        source.WithClause<TResult>(new SummarizeClause(aggregation, null));

    /// <summary>Produces aggregated values grouped by the specified key(s). KQL: <c>| summarize aggregation by groupBy</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="groupBy">An expression defining the group-by columns.</param>
    /// <param name="aggregation">An expression defining the aggregation functions.</param>
    public static KqlQuery<TResult> Summarize<T, TKey, TResult>(
        this KqlQuery<T> source,
        Expression<Func<T, TKey>> groupBy,
        Expression<Func<T, TResult>> aggregation) =>
        source.WithClause<TResult>(new SummarizeClause(aggregation, groupBy));

    /// <summary>Joins two tables on matching keys. KQL: <c>| join kind=X (inner) on key</c>.</summary>
    /// <param name="outer">The left/outer query.</param>
    /// <param name="inner">The right/inner query.</param>
    /// <param name="outerKey">The key selector for the outer table.</param>
    /// <param name="innerKey">The key selector for the inner table.</param>
    /// <param name="resultSelector">An expression defining the output shape.</param>
    /// <param name="kind">The join flavor (default: <see cref="JoinKind.InnerUnique"/>).</param>
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

    /// <summary>Merges rows from multiple tables with the same schema. KQL: <c>| union tables</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="others">The other queries to union with.</param>
    public static KqlQuery<T> Union<T>(this KqlQuery<T> source, params KqlQuery<T>[] others)
    {
        var otherKqls = others.Select(o => o.ToKql()).ToArray();
        return source.WithClause(new UnionClause(otherKqls, null));
    }

    /// <summary>Merges rows from multiple tables with the specified union kind. KQL: <c>| union kind=X tables</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="kind">The union kind (<see cref="UnionKind.Inner"/> or <see cref="UnionKind.Outer"/>).</param>
    /// <param name="others">The other queries to union with.</param>
    public static KqlQuery<T> Union<T>(this KqlQuery<T> source, UnionKind kind, params KqlQuery<T>[] others)
    {
        var otherKqls = others.Select(o => o.ToKql()).ToArray();
        return source.WithClause(new UnionClause(otherKqls, kind));
    }

    /// <summary>Expands a dynamic array column into multiple rows, one per array element. KQL: <c>| mv-expand column</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="arrayColumn">An expression selecting the array column to expand.</param>
    public static KqlQuery<T> MvExpand<T>(this KqlQuery<T> source, Expression<Func<T, object>> arrayColumn) =>
        source.WithClause(new MvExpandClause(arrayColumn));

    /// <summary>Applies a subquery to each element of a dynamic array column (inverse of <c>mv-expand</c>). KQL: <c>| mv-apply column on (subquery)</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="arrayColumn">An expression selecting the array column.</param>
    /// <param name="innerPipeline">A function that builds the inner subquery pipeline.</param>
    public static KqlQuery<T> MvApply<T, TApply>(
        this KqlQuery<T> source,
        Expression<Func<T, object>> arrayColumn,
        Func<KqlQuery<T>, KqlQuery<TApply>> innerPipeline)
    {
        var inner = new KqlQuery<T>("");
        var built = innerPipeline(inner);
        var innerKql = built.ToKql();
        return source.WithClause(new MvApplyClause(arrayColumn, innerKql));
    }

    /// <summary>
    /// Scans records using a state machine with declared state columns and step definitions.
    /// KQL: <c>| scan [with_match_id=id] [declare (...)] with (step ...)</c>.
    /// </summary>
    /// <param name="source">The source query.</param>
    /// <param name="configure">An action that configures the scan builder.</param>
    /// <example>
    /// <code>
    /// query.Scan(b => b
    ///     .Declare("InSession:bool = false")
    ///     .WithStep("start", "EventType == 'Login'", "InSession = true")
    ///     .WithStep("end", "EventType == 'Logout' and InSession", "InSession = false"));
    /// </code>
    /// </example>
    public static KqlQuery<T> Scan<T>(this KqlQuery<T> source, Action<KqlScanBuilder> configure)
    {
        var builder = new KqlScanBuilder();
        configure(builder);
        return source.WithClause(new ScanClause(builder.Build()));
    }

    /// <summary>Partitions the input by a column and applies a subquery to each partition independently. KQL: <c>| partition by column (subquery)</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="column">An expression selecting the partitioning column.</param>
    /// <param name="innerPipeline">A function that builds the inner subquery pipeline.</param>
    public static KqlQuery<T> PartitionBy<T, TApply>(
        this KqlQuery<T> source,
        Expression<Func<T, object>> column,
        Func<KqlQuery<T>, KqlQuery<TApply>> innerPipeline)
    {
        var inner = new KqlQuery<T>("");
        var built = innerPipeline(inner);
        var innerKql = built.ToKql();
        return source.WithClause(new PartitionByClause(column, innerKql));
    }

    /// <summary>Parses a string column using a pattern. KQL: <c>| parse column with pattern</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="column">The string column to parse.</param>
    /// <param name="pattern">The parse pattern (e.g. <c>"Error: " ErrorMsg:string " at " Location:string</c>).</param>
    public static KqlQuery<TResult> Parse<T, TResult>(this KqlQuery<T> source, Expression<Func<T, string>> column, string pattern) =>
        source.WithClause<TResult>(new ParseClause(column, pattern));

    /// <summary>Unpacks a dynamic property bag column into individual columns. KQL: <c>| evaluate bag_unpack(column)</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="column">The dynamic column to unpack.</param>
    public static KqlQuery<TResult> BagUnpack<T, TResult>(this KqlQuery<T> source, Expression<Func<T, object>> column) =>
        source.WithClause<TResult>(new BagUnpackClause(column));

    /// <summary>Requests a specific visualization type for the query results. KQL: <c>| render kind</c>.</summary>
    /// <param name="source">The source query.</param>
    /// <param name="kind">The visualization type.</param>
    public static KqlQuery<T> Render<T>(this KqlQuery<T> source, RenderKind kind) =>
        source.WithClause(new RenderClause(kind));
}
