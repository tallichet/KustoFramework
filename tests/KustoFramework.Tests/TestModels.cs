using KustoFramework.Attributes;

namespace KustoFramework.Tests;

[KqlTable("StormEvents")]
public class StormEvent
{
    public DateTime StartTime { get; set; }
    [KqlColumn("State")]
    public string State { get; set; } = "";
    public string EventType { get; set; } = "";
    public int DamageProperty { get; set; }
    public string Source { get; set; } = "";
    public int DeathsDirect { get; set; }
    public int InjuriesDirect { get; set; }
    public double BeginLat { get; set; }
    public double BeginLon { get; set; }
    public string EpisodeNarrative { get; set; } = "";
    public string[] Tags { get; set; } = [];
    public object DynamicBag { get; set; } = new();
}

[KqlTable("PopulationData")]
public class PopulationData
{
    public string State { get; set; } = "";
    public long Population { get; set; }
}

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = "";
    public string Message { get; set; } = "";
    public string RawMessage { get; set; } = "";
    public int Duration { get; set; }
    public string Region { get; set; } = "";
    public double Amount { get; set; }
    public string Category { get; set; } = "";
}
