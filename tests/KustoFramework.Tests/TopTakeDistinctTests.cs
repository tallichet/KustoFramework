using KustoFramework.Enums;
using KustoFramework.Extensions;

namespace KustoFramework.Tests;

public class TopTakeDistinctTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void Top_Descending()
    {
        var kql = _ctx.Table<StormEvent>()
            .Top(10, e => e.DamageProperty)
            .ToKql();

        Assert.Equal("StormEvents\n| top 10 by DamageProperty desc", kql);
    }

    [Fact]
    public void Top_Ascending()
    {
        var kql = _ctx.Table<StormEvent>()
            .Top(5, e => e.StartTime, SortOrder.Ascending)
            .ToKql();

        Assert.Equal("StormEvents\n| top 5 by StartTime asc", kql);
    }

    [Fact]
    public void Take()
    {
        var kql = _ctx.Table<StormEvent>()
            .Take(100)
            .ToKql();

        Assert.Equal("StormEvents\n| take 100", kql);
    }

    [Fact]
    public void Distinct_NoSelector()
    {
        var kql = _ctx.Table<StormEvent>()
            .Distinct()
            .ToKql();

        Assert.Equal("StormEvents\n| distinct", kql);
    }

    [Fact]
    public void Distinct_WithSelector()
    {
        var kql = _ctx.Table<StormEvent>()
            .Distinct(e => new { e.State, e.EventType })
            .ToKql();

        Assert.Equal("StormEvents\n| distinct State, EventType", kql);
    }

    [Fact]
    public void Count_Terminal()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State == "TEXAS")
            .Count()
            .ToKql();

        Assert.Equal("StormEvents\n| where State == \"TEXAS\"\n| count", kql);
    }
}
