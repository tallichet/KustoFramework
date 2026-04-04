using KustoFramework.Extensions;
using KustoFramework.Functions;

namespace KustoFramework.Tests;

public class PartitionByTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void PartitionBy_WithTop()
    {
        var kql = _ctx.Table<StormEvent>()
            .PartitionBy(
                e => e.State,
                inner => inner.Top(3, e => e.DamageProperty))
            .ToKql();

        Assert.Equal("StormEvents\n| partition by State (\n| top 3 by DamageProperty desc)", kql);
    }

    [Fact]
    public void PartitionBy_WithSummarize()
    {
        var kql = _ctx.Table<StormEvent>()
            .PartitionBy(
                e => e.State,
                inner => inner.Summarize(e => new { Total = Kql.Count() }))
            .ToKql();

        Assert.Equal("StormEvents\n| partition by State (\n| summarize Total = count())", kql);
    }

    [Fact]
    public void PartitionBy_WithWhereAndTake()
    {
        var kql = _ctx.Table<StormEvent>()
            .PartitionBy(
                e => e.State,
                inner => inner
                    .Where(e => e.DamageProperty > 0)
                    .Take(5))
            .ToKql();

        Assert.Equal("StormEvents\n| partition by State (\n| where DamageProperty > 0\n| take 5)", kql);
    }
}
