# Advanced Usage

## Complex Chained Queries

KustoFramework queries are immutable — each operator returns a new query. You can compose complex pipelines naturally:

```csharp
var context = new KustoContext();

string kql = context.Table<StormEvent>()
    .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromDays(365)))
    .Where(e => e.DamageProperty > 0)
    .Summarize(
        groupBy: e => e.State,
        aggregation: e => new { Count = Kql.Count(), TotalDamage = Kql.Sum(e.DamageProperty) })
    .Top(10, x => x.TotalDamage)
    .Render(RenderKind.BarChart)
    .ToKql();
```

Produces:

```kql
StormEvents
| where StartTime > ago(365d)
| where DamageProperty > 0
| summarize Count = count(), TotalDamage = sum(DamageProperty) by State
| top 10 by TotalDamage desc
| render barchart
```

## Reusing Query Fragments

Since queries are immutable, you can build reusable fragments:

```csharp
var baseQuery = context.Table<StormEvent>()
    .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromDays(30)));

// Branch into different queries
string countKql = baseQuery.Count().ToKql();
string topDamage = baseQuery.Top(5, e => e.DamageProperty).ToKql();
```

## Aggregation Patterns

### Time-bucketed aggregation

```csharp
context.Table<StormEvent>()
    .Summarize(
        groupBy: e => Kql.Bin(e.StartTime, TimeSpan.FromDays(1)),
        aggregation: e => new { Count = Kql.Count() })
    .Render(RenderKind.TimeChart)
    .ToKql();
// | summarize Count = count() by bin(StartTime, 1d)
// | render timechart
```

### Multiple aggregations with multiple group-by keys

```csharp
context.Table<StormEvent>()
    .Summarize(
        groupBy: e => new { e.State, e.EventType },
        aggregation: e => new
        {
            Count = Kql.Count(),
            AvgDamage = Kql.Avg(e.DamageProperty),
            MaxDamage = Kql.Max(e.DamageProperty)
        })
    .ToKql();
// | summarize Count = count(), AvgDamage = avg(DamageProperty), MaxDamage = max(DamageProperty) by State, EventType
```

### Group by time function

```csharp
context.Table<StormEvent>()
    .Summarize(
        groupBy: e => Kql.StartOfMonth(e.StartTime),
        aggregation: e => new { Count = Kql.Count() })
    .ToKql();
// | summarize Count = count() by startofmonth(StartTime)
```

## Join Kinds

KustoFramework supports all KQL join kinds:

| JoinKind | KQL |
|---|---|
| `JoinKind.InnerUnique` | `kind=innerunique` (default) |
| `JoinKind.Inner` | `kind=inner` |
| `JoinKind.LeftOuter` | `kind=leftouter` |
| `JoinKind.RightOuter` | `kind=rightouter` |
| `JoinKind.FullOuter` | `kind=fullouter` |
| `JoinKind.LeftSemi` | `kind=leftsemi` |
| `JoinKind.LeftAnti` | `kind=leftanti` |
| `JoinKind.RightSemi` | `kind=rightsemi` |
| `JoinKind.RightAnti` | `kind=rightanti` |

### Join with filtered inner table

```csharp
var storms = context.Table<StormEvent>();
var population = context.Table<PopulationData>()
    .Where(p => p.Population > 1_000_000);

storms.Join(
    population,
    outerKey: e => e.State,
    innerKey: p => p.State,
    resultSelector: (e, p) => new { e.EventType, p.Population },
    kind: JoinKind.Inner)
    .ToKql();
// | join kind=inner (PopulationData | where Population > 1000000) on State
// | project EventType, Population
```

## KQL-Specific Operators

### mv-expand

Expands an array (or dynamic) column into individual rows:

```csharp
context.Table<StormEvent>()
    .MvExpand(e => e.Tags)
    .Where(e => e.State == "TEXAS")
    .ToKql();
// StormEvents
// | mv-expand Tags
// | where State == "TEXAS"
```

### parse

Extract structured data from a string:

```csharp
context.Table<LogEntry>()
    .Parse<LogEntry, object>(e => e.RawMessage, "\"Error: \" ErrorMsg:string \" at \" Location:string")
    .ToKql();
// LogEntry
// | parse RawMessage with "Error: " ErrorMsg:string " at " Location:string
```

### evaluate bag_unpack

Unpack a dynamic/bag column into individual columns:

```csharp
context.Table<StormEvent>()
    .BagUnpack<StormEvent, object>(e => e.DynamicBag)
    .ToKql();
// StormEvents
// | evaluate bag_unpack(DynamicBag)
```

## String Operator Performance Guidance

When filtering strings in KQL, operator choice significantly impacts query performance. From fastest to slowest:

1. **`has`** — Term-based lookup (uses inverted index). **Fastest.** Use when searching for complete terms.
2. **`has_cs`** — Case-sensitive variant of `has`.
3. **`hasprefix` / `hassuffix`** — Prefix/suffix term lookup.
4. **`==`** — Exact equality (uses index for short strings).
5. **`startswith` / `endswith`** — Prefix/suffix substring match.
6. **`contains`** — Substring search. **Slower** — full column scan.
7. **`matches regex`** — Regular expression. **Slowest** — full column scan with regex engine.

### Recommendation

Prefer `KqlHas()` over `Contains()` when searching for whole terms:

```csharp
// FAST: uses inverted index
.Where(e => e.Source.KqlHas("newspaper"))

// SLOW: substring scan
.Where(e => e.Source.Contains("newspaper"))
```

## Captured Variables and Closures

C# variables captured in lambda expressions are evaluated when `.ToKql()` is called:

```csharp
var threshold = 1000;
var states = new[] { "TEXAS", "FLORIDA" };

context.Table<StormEvent>()
    .Where(e => e.DamageProperty > threshold)
    .Where(e => states.Contains(e.State))
    .ToKql();
// | where DamageProperty > 1000
// | where State in ("TEXAS", "FLORIDA")
```

## Limitations

- **No `let` statements** — Deferred to V2.
- **No `KqlDynamic` type** — For KQL bag/dynamic columns, a special helper type will be added later.
- **Runtime execution** — V1 generates KQL strings only. Actual execution against a Kusto cluster will be available in `KustoFramework.Client` (V2).
- **Expression complexity** — Very complex nested expressions may not translate. Keep individual expressions readable.
- **`mv-apply`** — Basic support. Complex subquery patterns may require raw KQL.
