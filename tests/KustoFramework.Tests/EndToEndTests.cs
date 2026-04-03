using KustoFramework.Enums;
using KustoFramework.Extensions;
using KustoFramework.Functions;

namespace KustoFramework.Tests;

public class EndToEndTests
{
    private readonly KustoContext _context = new();

    [Fact]
    public void WhereProjectPipeline()
    {
        var kql = _context.Table<StormEvent>()
            .Where(e => e.State == "TEXAS" && e.StartTime > Kql.Ago(TimeSpan.FromDays(7)))
            .Project(e => new { e.StartTime, e.EventType, e.DamageProperty })
            .ToKql();

        Assert.Equal(
            "StormEvents\n| where State == \"TEXAS\" and StartTime > ago(7d)\n| project StartTime, EventType, DamageProperty",
            kql);
    }

    [Fact]
    public void SummarizeTopPipeline()
    {
        var kql = _context.Table<StormEvent>()
            .Summarize(
                groupBy: e => e.State,
                aggregation: e => new { Count = Kql.Count(), TotalDamage = Kql.Sum(e.DamageProperty) })
            .Top(10, x => x.TotalDamage)
            .ToKql();

        Assert.Equal(
            "StormEvents\n| summarize Count = count(), TotalDamage = sum(DamageProperty) by State\n| top 10 by TotalDamage desc",
            kql);
    }

    [Fact]
    public void WhereExtendOrderByTake()
    {
        var kql = _context.Table<StormEvent>()
            .Where(e => e.DamageProperty > 0)
            .Extend(e => new { TotalCasualties = e.DeathsDirect + e.InjuriesDirect })
            .OrderByDescending(e => e.TotalCasualties)
            .Take(50)
            .ToKql();

        Assert.Equal(
            "StormEvents\n| where DamageProperty > 0\n| extend TotalCasualties = DeathsDirect + InjuriesDirect\n| sort by TotalCasualties desc\n| take 50",
            kql);
    }

    [Fact]
    public void WhereCountPipeline()
    {
        var kql = _context.Table<StormEvent>()
            .Where(e => e.State == "TEXAS")
            .Where(e => e.EventType == "Tornado")
            .Count()
            .ToKql();

        Assert.Equal(
            "StormEvents\n| where State == \"TEXAS\"\n| where EventType == \"Tornado\"\n| count",
            kql);
    }

    [Fact]
    public void SummarizeWithBinGroupBy()
    {
        var kql = _context.Table<StormEvent>()
            .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromDays(30)))
            .Summarize(
                groupBy: e => Kql.Bin(e.StartTime, TimeSpan.FromDays(1)),
                aggregation: e => new { Count = Kql.Count() })
            .Render(RenderKind.TimeChart)
            .ToKql();

        Assert.Equal(
            "StormEvents\n| where StartTime > ago(30d)\n| summarize Count = count() by bin(StartTime, 1d)\n| render timechart",
            kql);
    }

    [Fact]
    public void DistinctProject()
    {
        var kql = _context.Table<StormEvent>()
            .Where(e => e.DamageProperty > 0)
            .Distinct(e => new { e.State, e.EventType })
            .ToKql();

        Assert.Equal(
            "StormEvents\n| where DamageProperty > 0\n| distinct State, EventType",
            kql);
    }

    [Fact]
    public void JoinWithFilterAndProject()
    {
        var storms = _context.Table<StormEvent>().Where(e => e.DamageProperty > 0);
        var population = _context.Table<PopulationData>();

        var kql = storms
            .Join(
                population,
                outerKey: e => e.State,
                innerKey: p => p.State,
                resultSelector: (e, p) => new { e.State, e.EventType, p.Population },
                kind: JoinKind.LeftOuter)
            .ToKql();

        Assert.Contains("| where DamageProperty > 0", kql);
        Assert.Contains("| join kind=leftouter (PopulationData) on State", kql);
        Assert.Contains("| project State, EventType, Population", kql);
    }

    [Fact]
    public void StringOperatorsInPipeline()
    {
        var kql = _context.Table<StormEvent>()
            .Where(e => e.State.KqlHas("TEXAS"))
            .Where(e => e.EventType.Contains("Tornado"))
            .Project(e => new { e.StartTime, e.State, e.EventType })
            .Take(10)
            .ToKql();

        Assert.Equal(
            "StormEvents\n| where State has \"TEXAS\"\n| where EventType contains \"Tornado\"\n| project StartTime, State, EventType\n| take 10",
            kql);
    }

    [Fact]
    public void MvExpandWithWhereAndProject()
    {
        var kql = _context.Table<StormEvent>()
            .MvExpand(e => e.Tags)
            .Where(e => e.State == "TEXAS")
            .Project(e => new { e.State, e.Tags })
            .ToKql();

        Assert.Equal(
            "StormEvents\n| mv-expand Tags\n| where State == \"TEXAS\"\n| project State, Tags",
            kql);
    }

    [Fact]
    public void KustoContext_ResolvesTableName()
    {
        var query = _context.Table<StormEvent>();
        Assert.Equal("StormEvents", query.ToKql());
    }

    [Fact]
    public void KustoContext_CustomTableName()
    {
        var query = _context.Table<StormEvent>("CustomTable");
        Assert.Equal("CustomTable", query.ToKql());
    }

    [Fact]
    public void ComplexMultiOperator()
    {
        var kql = _context.Table<StormEvent>()
            .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromDays(365)))
            .Where(e => e.DamageProperty > 0)
            .Summarize(
                groupBy: e => e.State,
                aggregation: e => new { Count = Kql.Count(), TotalDamage = Kql.Sum(e.DamageProperty) })
            .Top(10, x => x.TotalDamage)
            .Render(RenderKind.BarChart)
            .ToKql();

        Assert.Equal(
            "StormEvents\n| where StartTime > ago(365d)\n| where DamageProperty > 0\n| summarize Count = count(), TotalDamage = sum(DamageProperty) by State\n| top 10 by TotalDamage desc\n| render barchart",
            kql);
    }
}
