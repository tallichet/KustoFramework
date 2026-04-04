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

    [Fact]
    public void Summarize_TakeAny()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => e.State,
                aggregation: e => new { AnyEvent = Kql.TakeAny(e.EventType) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize AnyEvent = take_any(EventType) by State", kql);
    }

    [Fact]
    public void Summarize_Stdev()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(e => new { StdDev = Kql.Stdev(e.DamageProperty) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize StdDev = stdev(DamageProperty)", kql);
    }

    [Fact]
    public void Summarize_Variance()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(e => new { Var = Kql.Variance(e.DamageProperty) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Var = variance(DamageProperty)", kql);
    }

    [Fact]
    public void Summarize_Percentiles()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(e => new { Pcts = Kql.Percentiles(e.DamageProperty, 50, 90, 99) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Pcts = percentiles(DamageProperty, 50, 90, 99)", kql);
    }

    [Fact]
    public void Summarize_MakeBag()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(e => new { Bag = Kql.MakeBag(e.DynamicBag) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Bag = make_bag(DynamicBag)", kql);
    }

    [Fact]
    public void Summarize_MakeListWithMaxSize()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => e.State,
                aggregation: e => new { Top10 = Kql.MakeList(e.EventType, 10) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Top10 = make_list(EventType, 10) by State", kql);
    }

    [Fact]
    public void Summarize_MakeSetWithMaxSize()
    {
        var kql = _ctx.Table<StormEvent>()
            .Summarize(
                groupBy: e => e.State,
                aggregation: e => new { Top5 = Kql.MakeSet(e.EventType, 5) })
            .ToKql();

        Assert.Equal("StormEvents\n| summarize Top5 = make_set(EventType, 5) by State", kql);
    }
}
