using KustoFramework.Attributes;

namespace KustoFramework.Azure.Tests;

[KqlTable("StormEvents")]
public class StormEvent
{
    public DateTime StartTime { get; set; }
    [KqlColumn("State")]
    public string State { get; set; } = "";
    public string EventType { get; set; } = "";
    public int DamageProperty { get; set; }
    public int DeathsDirect { get; set; }
    public int InjuriesDirect { get; set; }
}

public class SimpleRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsActive { get; set; }
    public long Count { get; set; }
    public Guid TraceId { get; set; }
}

public class NullableRecord
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public double? Value { get; set; }
    public DateTime? Timestamp { get; set; }
}

[KqlTable("CustomTable")]
public class MappedRecord
{
    [KqlColumn("record_id")]
    public int Id { get; set; }

    [KqlColumn("display_name")]
    public string Name { get; set; } = "";
}
