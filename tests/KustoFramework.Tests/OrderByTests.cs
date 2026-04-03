using KustoFramework.Enums;
using KustoFramework.Extensions;

namespace KustoFramework.Tests;

public class OrderByTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void OrderBy_Ascending()
    {
        var kql = _ctx.Table<StormEvent>()
            .OrderBy(e => e.StartTime)
            .ToKql();

        Assert.Equal("StormEvents\n| sort by StartTime asc", kql);
    }

    [Fact]
    public void OrderByDescending()
    {
        var kql = _ctx.Table<StormEvent>()
            .OrderByDescending(e => e.DamageProperty)
            .ToKql();

        Assert.Equal("StormEvents\n| sort by DamageProperty desc", kql);
    }

    [Fact]
    public void OrderBy_ThenBy()
    {
        var kql = _ctx.Table<StormEvent>()
            .OrderByDescending(e => e.DamageProperty)
            .ThenBy(e => e.State)
            .ToKql();

        Assert.Equal("StormEvents\n| sort by DamageProperty desc, State asc", kql);
    }

    [Fact]
    public void OrderBy_MultipleThenBy()
    {
        var kql = _ctx.Table<StormEvent>()
            .OrderBy(e => e.State)
            .ThenByDescending(e => e.StartTime)
            .ThenBy(e => e.EventType)
            .ToKql();

        Assert.Equal("StormEvents\n| sort by State asc, StartTime desc, EventType asc", kql);
    }
}
