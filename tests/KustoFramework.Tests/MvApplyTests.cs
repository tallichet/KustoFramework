using KustoFramework.Extensions;
using KustoFramework.Functions;

namespace KustoFramework.Tests;

public class MvApplyTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void MvApply_WithSummarize()
    {
        var kql = _ctx.Table<StormEvent>()
            .MvApply(
                e => e.Tags,
                inner => inner.Summarize(e => new { TagCount = Kql.Count() }))
            .ToKql();

        Assert.Equal("StormEvents\n| mv-apply Tags on (\n| summarize TagCount = count())", kql);
    }

    [Fact]
    public void MvApply_WithWhere()
    {
        var kql = _ctx.Table<StormEvent>()
            .MvApply(
                e => e.Tags,
                inner => inner.Where(e => e.DamageProperty > 100))
            .ToKql();

        Assert.Equal("StormEvents\n| mv-apply Tags on (\n| where DamageProperty > 100)", kql);
    }

    [Fact]
    public void MvApply_WithMultipleOperators()
    {
        var kql = _ctx.Table<StormEvent>()
            .MvApply(
                e => e.Tags,
                inner => inner
                    .Where(e => Kql.IsNotEmpty(e.EventType))
                    .Summarize(e => new { Total = Kql.Count() }))
            .ToKql();

        Assert.Equal("StormEvents\n| mv-apply Tags on (\n| where isnotempty(EventType)\n| summarize Total = count())", kql);
    }
}
