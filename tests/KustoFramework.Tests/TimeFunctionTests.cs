using KustoFramework.Extensions;
using KustoFramework.Functions;

namespace KustoFramework.Tests;

public class TimeFunctionTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void Where_Ago_Days()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromDays(1)))
            .ToKql();

        Assert.Equal("StormEvents\n| where StartTime > ago(1d)", kql);
    }

    [Fact]
    public void Where_Ago_Hours()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromHours(4)))
            .ToKql();

        Assert.Equal("StormEvents\n| where StartTime > ago(4h)", kql);
    }

    [Fact]
    public void Where_Ago_Minutes()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromMinutes(30)))
            .ToKql();

        Assert.Equal("StormEvents\n| where StartTime > ago(30m)", kql);
    }

    [Fact]
    public void Summarize_Bin()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => Kql.Bin(e.StartTime, TimeSpan.FromHours(1)),
                aggregation: e => new { Total = Kql.Count() })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Total = count() by bin(StartTime, 1h)", kql);
    }

    [Fact]
    public void Where_Now()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.StartTime < Kql.Now())
            .ToKql();

        Assert.Equal("StormEvents\n| where StartTime < now()", kql);
    }

    [Fact]
    public void Summarize_StartOfDay()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => Kql.StartOfDay(e.StartTime),
                aggregation: e => new { Total = Kql.Count() })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Total = count() by startofday(StartTime)", kql);
    }

    [Fact]
    public void Summarize_StartOfMonth()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => Kql.StartOfMonth(e.StartTime),
                aggregation: e => new { Total = Kql.Count() })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Total = count() by startofmonth(StartTime)", kql);
    }
}
