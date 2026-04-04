using KustoFramework.Extensions;
using KustoFramework.Functions;

namespace KustoFramework.Tests;

public class ScalarFunctionTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void Extract_RegexFromString()
    {
        var kql = _ctx.Table<StormEvent>()
            .Extend(e => new { Code = Kql.Extract("([A-Z]{2})-([0-9]+)", 1, e.Source) })
            .ToKql();

        Assert.Equal("StormEvents\n| extend Code = extract(\"([A-Z]{2})-([0-9]+)\", 1, Source)", kql);
    }

    [Fact]
    public void Split_String()
    {
        var kql = _ctx.Table<StormEvent>()
            .Extend(e => new { Parts = Kql.Split(e.Source, ",") })
            .ToKql();

        Assert.Equal("StormEvents\n| extend Parts = split(Source, \",\")", kql);
    }

    [Fact]
    public void ReplaceString_InExtend()
    {
        var kql = _ctx.Table<StormEvent>()
            .Extend(e => new { CleanSource = Kql.ReplaceString(e.Source, "old", "new") })
            .ToKql();

        Assert.Equal("StormEvents\n| extend CleanSource = replace_string(Source, \"old\", \"new\")", kql);
    }

    [Fact]
    public void ReplaceRegex_InExtend()
    {
        var kql = _ctx.Table<StormEvent>()
            .Extend(e => new { Clean = Kql.ReplaceRegex(e.Source, "[0-9]+", "N") })
            .ToKql();

        Assert.Equal("StormEvents\n| extend Clean = replace_regex(Source, \"[0-9]+\", \"N\")", kql);
    }

    [Fact]
    public void IndexOf_InExtend()
    {
        var kql = _ctx.Table<StormEvent>()
            .Extend(e => new { Pos = Kql.IndexOf(e.Source, "news") })
            .ToKql();

        Assert.Equal("StormEvents\n| extend Pos = indexof(Source, \"news\")", kql);
    }

    [Fact]
    public void ParseJson_InExtend()
    {
        var kql = _ctx.Table<StormEvent>()
            .Extend(e => new { Parsed = Kql.ParseJson(e.EpisodeNarrative) })
            .ToKql();

        Assert.Equal("StormEvents\n| extend Parsed = parse_json(EpisodeNarrative)", kql);
    }

    [Fact]
    public void ArrayLength_InExtend()
    {
        var kql = _ctx.Table<StormEvent>()
            .Extend(e => new { Len = Kql.ArrayLength(e.Tags) })
            .ToKql();

        Assert.Equal("StormEvents\n| extend Len = array_length(Tags)", kql);
    }

    [Fact]
    public void BagKeys_InExtend()
    {
        var kql = _ctx.Table<StormEvent>()
            .Extend(e => new { Keys = Kql.BagKeys(e.DynamicBag) })
            .ToKql();

        Assert.Equal("StormEvents\n| extend Keys = bag_keys(DynamicBag)", kql);
    }
}
