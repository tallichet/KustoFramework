using KustoFramework.Extensions;
using KustoFramework.Enums;

namespace KustoFramework.Tests;

public class MvExpandTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void MvExpand_Column()
    {
        var kql = _ctx.Table<StormEvent>()
            .MvExpand(e => e.Tags)
            .ToKql();

        Assert.Equal("StormEvents\n| mv-expand Tags", kql);
    }

    [Fact]
    public void Render_TimeChart()
    {
        var kql = _ctx.Table<StormEvent>()
            .Render(RenderKind.TimeChart)
            .ToKql();

        Assert.Equal("StormEvents\n| render timechart", kql);
    }

    [Fact]
    public void Render_BarChart()
    {
        var kql = _ctx.Table<StormEvent>()
            .Render(RenderKind.BarChart)
            .ToKql();

        Assert.Equal("StormEvents\n| render barchart", kql);
    }

    [Fact]
    public void Parse_StringColumn()
    {
        var kql = _ctx.Table<LogEntry>()
            .Parse<LogEntry, object>(e => e.RawMessage, "\"Error: \" ErrorMsg:string \" at \" Location:string")
            .ToKql();

        Assert.Equal("LogEntry\n| parse RawMessage with \"Error: \" ErrorMsg:string \" at \" Location:string", kql);
    }

    [Fact]
    public void BagUnpack_Column()
    {
        var kql = _ctx.Table<StormEvent>()
            .BagUnpack<StormEvent, object>(e => e.DynamicBag)
            .ToKql();

        Assert.Equal("StormEvents\n| evaluate bag_unpack(DynamicBag)", kql);
    }
}
