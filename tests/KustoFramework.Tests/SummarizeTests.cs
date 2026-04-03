using KustoFramework.Extensions;
using KustoFramework.Functions;

namespace KustoFramework.Tests;

public class SummarizeTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void Summarize_Count_NoGroupBy()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(e => new { Total = Kql.Count() })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Total = count()", kql);
    }

    [Fact]
    public void Summarize_Count_WithGroupBy()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => e.State,
                aggregation: e => new { Total = Kql.Count() })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Total = count() by State", kql);
    }

    [Fact]
    public void Summarize_Sum_WithGroupBy()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => e.State,
                aggregation: e => new { TotalDamage = Kql.Sum(e.DamageProperty) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize TotalDamage = sum(DamageProperty) by State", kql);
    }

    [Fact]
    public void Summarize_MultipleAggregations_MultipleGroupBy()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => new { e.State, e.EventType },
                aggregation: e => new { Total = Kql.Count(), MaxDamage = Kql.Max(e.DamageProperty) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Total = count(), MaxDamage = max(DamageProperty) by State, EventType", kql);
    }

    [Fact]
    public void Summarize_Avg()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(e => new { AvgDamage = Kql.Avg(e.DamageProperty) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize AvgDamage = avg(DamageProperty)", kql);
    }

    [Fact]
    public void Summarize_DCount()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(e => new { UniqueStates = Kql.DCount(e.State) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize UniqueStates = dcount(State)", kql);
    }

    [Fact]
    public void Summarize_Min_Max()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(e => new { Earliest = Kql.Min(e.StartTime), Latest = Kql.Max(e.StartTime) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Earliest = min(StartTime), Latest = max(StartTime)", kql);
    }

    [Fact]
    public void Summarize_Percentile()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(e => new { P95 = Kql.Percentile(e.DamageProperty, 95.0) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize P95 = percentile(DamageProperty, 95)", kql);
    }

    [Fact]
    public void Summarize_MakeList()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => e.State,
                aggregation: e => new { Events = Kql.MakeList(e.EventType) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Events = make_list(EventType) by State", kql);
    }

    [Fact]
    public void Summarize_MakeSet()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => e.State,
                aggregation: e => new { UniqueEvents = Kql.MakeSet(e.EventType) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize UniqueEvents = make_set(EventType) by State", kql);
    }
}
