using System.Linq.Expressions;
using KustoFramework.Enums;

namespace KustoFramework.Query;

public abstract record KqlClause
{
    public abstract string ToKql(KqlExpressionVisitor visitor);
}

public sealed record WhereClause(LambdaExpression Predicate) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor) =>
        $"| where {visitor.Translate(Predicate)}";
}

public sealed record ProjectClause(LambdaExpression Selector) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor) =>
        $"| project {visitor.TranslateProjection(Selector)}";
}

public sealed record ProjectAwayClause(LambdaExpression[] Columns) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor) =>
        $"| project-away {string.Join(", ", Columns.Select(c => visitor.TranslateMemberAccess(c)))}";
}

public sealed record ExtendClause(LambdaExpression Extension) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor) =>
        $"| extend {visitor.TranslateProjection(Extension)}";
}

public sealed record OrderByClause(List<(LambdaExpression KeySelector, SortOrder Order)> Keys) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor)
    {
        var parts = Keys.Select(k =>
        {
            var col = visitor.TranslateMemberAccess(k.KeySelector);
            var dir = k.Order == SortOrder.Ascending ? "asc" : "desc";
            return $"{col} {dir}";
        });
        return $"| sort by {string.Join(", ", parts)}";
    }
}

public sealed record TopClause(int Count, LambdaExpression OrderBy, SortOrder Order) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor)
    {
        var col = visitor.TranslateMemberAccess(OrderBy);
        var dir = Order == SortOrder.Ascending ? "asc" : "desc";
        return $"| top {Count} by {col} {dir}";
    }
}

public sealed record TakeClause(int Count) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor) =>
        $"| take {Count}";
}

public sealed record DistinctClause(LambdaExpression? Selector) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor)
    {
        if (Selector is null)
            return "| distinct";

        return $"| distinct {visitor.TranslateProjection(Selector)}";
    }
}

public sealed record CountClause : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor) =>
        "| count";
}

public sealed record SummarizeClause(LambdaExpression Aggregation, LambdaExpression? GroupBy) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor)
    {
        var agg = visitor.TranslateProjection(Aggregation);
        if (GroupBy is null)
            return $"| summarize {agg}";

        var groupByKql = visitor.TranslateGroupBy(GroupBy);
        return $"| summarize {agg} by {groupByKql}";
    }
}

public sealed record JoinClause(string InnerKql, LambdaExpression OuterKey, LambdaExpression InnerKey, JoinKind Kind) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor)
    {
        var kind = Kind switch
        {
            JoinKind.InnerUnique => "innerunique",
            JoinKind.Inner => "inner",
            JoinKind.LeftOuter => "leftouter",
            JoinKind.RightOuter => "rightouter",
            JoinKind.FullOuter => "fullouter",
            JoinKind.LeftSemi => "leftsemi",
            JoinKind.LeftAnti => "leftanti",
            JoinKind.RightSemi => "rightsemi",
            JoinKind.RightAnti => "rightanti",
            _ => "innerunique"
        };

        var outerCol = visitor.TranslateMemberAccess(OuterKey);
        var innerCol = visitor.TranslateMemberAccess(InnerKey);

        var onClause = outerCol == innerCol
            ? outerCol
            : $"$left.{outerCol} == $right.{innerCol}";

        return $"| join kind={kind} ({InnerKql}) on {onClause}";
    }
}

public sealed record UnionClause(string[] OtherKqls, UnionKind? Kind) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor)
    {
        var kindStr = Kind.HasValue ? $" kind={Kind.Value.ToString().ToLowerInvariant()}" : "";
        return $"| union{kindStr} {string.Join(", ", OtherKqls)}";
    }
}

public sealed record MvExpandClause(LambdaExpression ArrayColumn) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor) =>
        $"| mv-expand {visitor.TranslateMemberAccess(ArrayColumn)}";
}

public sealed record ParseClause(LambdaExpression Column, string Pattern) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor) =>
        $"| parse {visitor.TranslateMemberAccess(Column)} with {Pattern}";
}

public sealed record BagUnpackClause(LambdaExpression Column) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor) =>
        $"| evaluate bag_unpack({visitor.TranslateMemberAccess(Column)})";
}

public sealed record RenderClause(RenderKind Kind) : KqlClause
{
    public override string ToKql(KqlExpressionVisitor visitor)
    {
        var kind = Kind switch
        {
            RenderKind.Table => "table",
            RenderKind.BarChart => "barchart",
            RenderKind.ColumnChart => "columnchart",
            RenderKind.PieChart => "piechart",
            RenderKind.TimeChart => "timechart",
            RenderKind.LineChart => "linechart",
            RenderKind.AreaChart => "areachart",
            RenderKind.StackedAreaChart => "stackedareachart",
            RenderKind.Ladder => "ladder",
            RenderKind.ScatterChart => "scatterchart",
            RenderKind.TreeMap => "treemap",
            RenderKind.Card => "card",
            _ => "table"
        };
        return $"| render {kind}";
    }
}
