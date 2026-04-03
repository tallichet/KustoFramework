using KustoFramework.Extensions;

namespace KustoFramework.Tests;

public class ProjectTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void Project_AnonymousType()
    {
        var kql = _ctx.Table<StormEvent>()
            .Project(e => new { e.State, e.EventType, e.DamageProperty })
            .ToKql();

        Assert.Equal("StormEvents\n| project State, EventType, DamageProperty", kql);
    }

    [Fact]
    public void Project_WithRename()
    {
        var kql = _ctx.Table<StormEvent>()
            .Project(e => new { Location = e.State, Type = e.EventType })
            .ToKql();

        Assert.Equal("StormEvents\n| project Location = State, Type = EventType", kql);
    }

    [Fact]
    public void Project_WithComputation()
    {
        var kql = _ctx.Table<StormEvent>()
            .Project(e => new { e.State, DoubleDamage = e.DamageProperty * 2 })
            .ToKql();

        Assert.Equal("StormEvents\n| project State, DoubleDamage = DamageProperty * 2", kql);
    }

    [Fact]
    public void ProjectAway_SingleColumn()
    {
        var kql = _ctx.Table<StormEvent>()
            .ProjectAway<StormEvent, string>(e => e.Source)
            .ToKql();

        Assert.Equal("StormEvents\n| project-away Source", kql);
    }

    [Fact]
    public void ProjectAway_MultipleColumns()
    {
        var kql = _ctx.Table<StormEvent>()
            .ProjectAway<StormEvent, object>(e => e.Source, e => e.EpisodeNarrative)
            .ToKql();

        Assert.Equal("StormEvents\n| project-away Source, EpisodeNarrative", kql);
    }
}
