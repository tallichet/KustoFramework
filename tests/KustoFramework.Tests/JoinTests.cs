using KustoFramework.Enums;
using KustoFramework.Extensions;

namespace KustoFramework.Tests;

public class JoinTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void Join_InnerUnique_SameKey()
    {
        var storms = _ctx.Table<StormEvent>();
        var pop = _ctx.Table<PopulationData>();

        var kql = storms
            .Join(
                pop,
                outerKey: e => e.State,
                innerKey: p => p.State,
                resultSelector: (e, p) => new { e.EventType, p.Population },
                kind: JoinKind.InnerUnique)
            .ToKql();

        Assert.Contains("| join kind=innerunique (PopulationData) on State", kql);
        Assert.Contains("| project EventType, Population", kql);
    }

    [Fact]
    public void Join_LeftOuter()
    {
        var storms = _ctx.Table<StormEvent>();
        var pop = _ctx.Table<PopulationData>();

        var kql = storms
            .Join(
                pop,
                outerKey: e => e.State,
                innerKey: p => p.State,
                resultSelector: (e, p) => new { e.State, e.EventType, p.Population },
                kind: JoinKind.LeftOuter)
            .ToKql();

        Assert.Contains("| join kind=leftouter (PopulationData) on State", kql);
    }

    [Fact]
    public void Join_WithFilteredInner()
    {
        var storms = _ctx.Table<StormEvent>();
        var pop = _ctx.Table<PopulationData>()
            .Where(p => p.Population > 1000000);

        var kql = storms
            .Join(
                pop,
                outerKey: e => e.State,
                innerKey: p => p.State,
                resultSelector: (e, p) => new { e.EventType, p.Population },
                kind: JoinKind.Inner)
            .ToKql();

        Assert.Contains("| join kind=inner (PopulationData\n| where Population > 1000000) on State", kql);
    }

    [Fact]
    public void Union_Basic()
    {
        var storms1 = _ctx.Table<StormEvent>().Where(e => e.State == "TEXAS");
        var storms2 = _ctx.Table<StormEvent>().Where(e => e.State == "FLORIDA");

        var kql = storms1.Union(storms2).ToKql();

        Assert.Contains("| union StormEvents\n| where State == \"FLORIDA\"", kql);
    }
}
