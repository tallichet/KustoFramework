using KustoFramework.Extensions;
using KustoFramework.Functions;

namespace KustoFramework.Tests;

public class StringFunctionTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void Where_StringContains()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.Source.Contains("newspaper"))
            .ToKql();

        Assert.Equal("StormEvents\n| where Source contains \"newspaper\"", kql);
    }

    [Fact]
    public void Where_StringStartsWith()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.EventType.StartsWith("Thunder"))
            .ToKql();

        Assert.Equal("StormEvents\n| where EventType startswith \"Thunder\"", kql);
    }

    [Fact]
    public void Where_StringEndsWith()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.EventType.EndsWith("Wind"))
            .ToKql();

        Assert.Equal("StormEvents\n| where EventType endswith \"Wind\"", kql);
    }

    [Fact]
    public void Where_KqlHas()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.Source.KqlHas("news"))
            .ToKql();

        Assert.Equal("StormEvents\n| where Source has \"news\"", kql);
    }

    [Fact]
    public void Where_KqlHasPrefix()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State.KqlHasPrefix("TEX"))
            .ToKql();

        Assert.Equal("StormEvents\n| where State hasprefix \"TEX\"", kql);
    }

    [Fact]
    public void Where_KqlHasSuffix()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State.KqlHasSuffix("AS"))
            .ToKql();

        Assert.Equal("StormEvents\n| where State hassuffix \"AS\"", kql);
    }

    [Fact]
    public void Where_KqlMatchesRegex()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State.KqlMatchesRegex("^T.*S$"))
            .ToKql();

        Assert.Equal("StormEvents\n| where State matches regex \"^T.*S$\"", kql);
    }

    [Fact]
    public void Where_KqlIn()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State.KqlIn("TEXAS", "FLORIDA", "CALIFORNIA"))
            .ToKql();

        Assert.Equal("StormEvents\n| where State in (\"TEXAS\", \"FLORIDA\", \"CALIFORNIA\")", kql);
    }

    [Fact]
    public void Where_KqlNotIn()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => e.State.KqlNotIn("TEXAS", "FLORIDA"))
            .ToKql();

        Assert.Equal("StormEvents\n| where State !in (\"TEXAS\", \"FLORIDA\")", kql);
    }

    [Fact]
    public void Where_IsEmpty()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => Kql.IsEmpty(e.Source))
            .ToKql();

        Assert.Equal("StormEvents\n| where isempty(Source)", kql);
    }

    [Fact]
    public void Where_IsNotEmpty()
    {
        var kql = _ctx.Table<StormEvent>()
            .Where(e => Kql.IsNotEmpty(e.Source))
            .ToKql();

        Assert.Equal("StormEvents\n| where isnotempty(Source)", kql);
    }
}
