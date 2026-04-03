using KustoFramework.Extensions;
using KustoFramework.Functions;

namespace KustoFramework.Tests;

public class WhereTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void Where_EqualityString()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State == "TEXAS")
            .ToKql();

        Assert.Equal("StormEvents\n| where State == \"TEXAS\"", kql);
    }

    [Fact]
    public void Where_EqualityInt()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.DamageProperty > 1000)
            .ToKql();

        Assert.Equal("StormEvents\n| where DamageProperty > 1000", kql);
    }

    [Fact]
    public void Where_AndCombination()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State == "TEXAS" && e.DamageProperty > 0)
            .ToKql();

        Assert.Equal("StormEvents\n| where State == \"TEXAS\" and DamageProperty > 0", kql);
    }

    [Fact]
    public void Where_OrCombination()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State == "TEXAS" || e.State == "FLORIDA")
            .ToKql();

        Assert.Equal("StormEvents\n| where State == \"TEXAS\" or State == \"FLORIDA\"", kql);
    }

    [Fact]
    public void Where_NotOperator()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => !(e.State == "TEXAS"))
            .ToKql();

        Assert.Equal("StormEvents\n| where not(State == \"TEXAS\")", kql);
    }

    [Fact]
    public void Where_WithAgo()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromDays(7)))
            .ToKql();

        Assert.Equal("StormEvents\n| where StartTime > ago(7d)", kql);
    }

    [Fact]
    public void Where_NullCheck()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.Source == null)
            .ToKql();

        Assert.Equal("StormEvents\n| where isnull(Source)", kql);
    }

    [Fact]
    public void Where_NotNullCheck()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.Source != null)
            .ToKql();

        Assert.Equal("StormEvents\n| where isnotnull(Source)", kql);
    }

    [Fact]
    public void Where_MultipleChained()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State == "TEXAS")
            .Where(e => e.DamageProperty > 0)
            .ToKql();

        Assert.Equal("StormEvents\n| where State == \"TEXAS\"\n| where DamageProperty > 0", kql);
    }

    [Fact]
    public void Where_CapturedVariable()
    {
        var state = "TEXAS";
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State == state)
            .ToKql();

        Assert.Equal("StormEvents\n| where State == \"TEXAS\"", kql);
    }

    [Fact]
    public void Where_ComplexAnd_Or()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State == "TEXAS" && (e.EventType == "Tornado" || e.EventType == "Hail"))
            .ToKql();

        Assert.Equal("StormEvents\n| where State == \"TEXAS\" and (EventType == \"Tornado\" or EventType == \"Hail\")", kql);
    }
}
