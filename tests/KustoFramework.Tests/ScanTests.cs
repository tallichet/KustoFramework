using KustoFramework.Extensions;
using KustoFramework.Query;

namespace KustoFramework.Tests;

public class ScanTests
{
    private readonly KustoContext _ctx = new();

    [Fact]
    public void Scan_WithDeclareAndSteps()
    {
        var kql = _ctx.Table<LogEntry>()
            .Scan(b => b
                .Declare("InSession:bool = false, SessionStart:datetime")
                .WithStep("start", "Level == 'Start'", "InSession = true, SessionStart = Timestamp")
                .WithStep("end", "Level == 'End' and InSession", "InSession = false"))
            .ToKql();

        Assert.Equal(
            "LogEntry\n| scan declare (InSession:bool = false, SessionStart:datetime) " +
            "with (step start: Level == 'Start' => InSession = true, SessionStart = Timestamp;, " +
            "step end: Level == 'End' and InSession => InSession = false;)",
            kql);
    }

    [Fact]
    public void Scan_WithMatchId()
    {
        var kql = _ctx.Table<LogEntry>()
            .Scan(b => b
                .WithMatchId("sessionId")
                .Declare("InSession:bool = false")
                .WithStep("start", "Level == 'Start'", "InSession = true"))
            .ToKql();

        Assert.Equal(
            "LogEntry\n| scan with_match_id=sessionId declare (InSession:bool = false) " +
            "with (step start: Level == 'Start' => InSession = true;)",
            kql);
    }

    [Fact]
    public void Scan_StepsWithoutOutput()
    {
        var kql = _ctx.Table<LogEntry>()
            .Scan(b => b
                .WithStep("match", "Level == 'Error'"))
            .ToKql();

        Assert.Equal("LogEntry\n| scan with (step match: Level == 'Error')", kql);
    }
}
