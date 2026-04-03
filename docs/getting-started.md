# Getting Started with KustoFramework# Getting Started with KustoFramework







































































































































```// | project EventType, Population// | join kind=leftouter (PopulationData) on State// StormEvents    .ToKql();        kind: JoinKind.LeftOuter)        resultSelector: (e, p) => new { e.EventType, p.Population },        innerKey: p => p.State,        outerKey: e => e.State,        population,    .Join(string kql = stormsvar population = context.Table<PopulationData>();var storms = context.Table<StormEvent>();```csharp### Joins```// | where EventType contains "Tornado"// | where Source has "newspaper"// StormEvents    .ToKql();    .Where(e => e.EventType.Contains("Tornado"))    .Where(e => e.Source.KqlHas("newspaper"))string kql = context.Table<StormEvent>()```csharp### String Operators```// | top 10 by TotalDamage desc// | summarize Count=count(), TotalDamage=sum(DamageProperty) by State// StormEvents    .ToKql();    .Top(10, x => x.TotalDamage)        aggregation: e => new { Count = Kql.Count(), TotalDamage = Kql.Sum(e.DamageProperty) })        groupBy: e => e.State,    .Summarize(string kql = context.Table<StormEvent>()```csharp### Aggregation with Summarize## More Examples| `KustoFramework.Enums` | `JoinKind`, `SortOrder`, `RenderKind`, `UnionKind` || `KustoFramework.Attributes` | `[KqlTable]` and `[KqlColumn]` attributes || `KustoFramework.Functions` | `Kql` static helper (aggregations, time) and `KqlStringExtensions` || `KustoFramework.Extensions` | All query extension methods (`Where`, `Project`, etc.) || `KustoFramework` | `KustoContext` entry point ||---|---|| Namespace | Purpose |## Namespaces3. **`.ToKql()`** walks the clause list, uses `KqlExpressionVisitor` to translate each C# expression tree into KQL syntax, and returns the final string.2. **Extension methods** (`.Where()`, `.Project()`, etc.) return new immutable `KqlQuery<T>` instances with accumulated clauses — just like LINQ.1. **`context.Table<T>()`** creates a `KqlQuery<T>` rooted at the table name.## How It Works```| project StartTime, EventType, DamageProperty| where State == "TEXAS" and StartTime > ago(7d)StormEvents```kqlThis produces:```    .ToKql();    .Project(e => new { e.StartTime, e.EventType, e.DamageProperty })    .Where(e => e.State == "TEXAS" && e.StartTime > Kql.Ago(TimeSpan.FromDays(7)))string kql = context.Table<StormEvent>()```csharp## Write Your First Query```var storms = context.Table<StormEvent>("StormEvents_2024");```csharpYou can also specify a custom table name:```var storms = context.Table<StormEvent>();var context = new KustoContext();using KustoFramework.Functions;using KustoFramework.Extensions;using KustoFramework;```csharp`KustoContext` is the entry point for building queries:## Create a KustoContext- `[KqlColumn("State")]` maps a property to a specific column name. If omitted, the property name is used.- `[KqlTable("StormEvents")]` maps the class to the `StormEvents` Kusto table. If omitted, the class name is used.```}    public int DeathsDirect { get; set; }    public string Source { get; set; } = "";    public int DamageProperty { get; set; }    public string EventType { get; set; } = "";    public string State { get; set; } = "";    [KqlColumn("State")]    public DateTime StartTime { get; set; }{public class StormEvent[KqlTable("StormEvents")]using KustoFramework.Attributes;```csharpMap your C# classes to Kusto tables using attributes:## Define Your POCO Types```<ProjectReference Include="path/to/src/KustoFramework/KustoFramework.csproj" />```xmlAdd the `KustoFramework` project reference to your application:## InstallationKustoFramework is a .NET library that generates KQL (Kusto Query Language) strings from POCO types using fluent extension methods, inspired by EF Core's `IQueryable` pattern. It uses expression trees to translate C# lambdas into type-safe KQL queries with deferred string generation.
KustoFramework is a .NET library that generates KQL (Kusto Query Language) strings from POCO types using fluent extension methods, inspired by EF Core's `IQueryable` pattern. It uses expression trees to translate C# lambdas into type-safe KQL queries with deferred string generation.

## Installation

Add the `KustoFramework` project reference to your solution. The library targets `net10.0` and has zero external dependencies.

## Defining Your Data Model

Map your C# classes to Kusto tables using attributes:

```csharp
using KustoFramework.Attributes;

[KqlTable("StormEvents")]
public class StormEvent
{
    public DateTime StartTime { get; set; }
    
    [KqlColumn("state")]
    public string State { get; set; } = "";
    
    public string EventType { get; set; } = "";
    public int DamageProperty { get; set; }
    public string Source { get; set; } = "";
}
```

- **`[KqlTable("TableName")]`** — Maps the class to a Kusto table. If omitted, the class name is used.
- **`[KqlColumn("ColumnName")]`** — Maps a property to a specific column name. If omitted, the property name is used as-is.

## Creating a Context and Querying

Use `KustoContext` as your entry point:

```csharp
using KustoFramework;
using KustoFramework.Extensions;
using KustoFramework.Functions;

var context = new KustoContext();
var storms = context.Table<StormEvent>();
```

### Simple Where + Project

```csharp
string kql = storms
    .Where(e => e.State == "TEXAS" && e.StartTime > Kql.Ago(TimeSpan.FromDays(7)))
    .Project(e => new { e.StartTime, e.EventType, e.DamageProperty })
    .ToKql();
```

Generates:

```kql
StormEvents
| where state == "TEXAS" and StartTime > ago(7d)
| project StartTime, EventType, DamageProperty
```

### Using `ToKql()`

Every query chain is immutable. Call `.ToKql()` at the end to generate the KQL string:

```csharp
// Build the query — no string is generated yet
var query = storms
    .Where(e => e.DamageProperty > 1000)
    .OrderByDescending(e => e.DamageProperty)
    .Take(10);

// Generate the KQL string
string kql = query.ToKql();
```

### Captured Variables

You can use local variables in expressions — they're evaluated at KQL generation time:

```csharp
var state = "TEXAS";
var minDamage = 500;

string kql = storms
    .Where(e => e.State == state && e.DamageProperty > minDamage)
    .ToKql();
// → StormEvents | where state == "TEXAS" and DamageProperty > 500
```

## Next Steps

- See [Operators Reference](operators-reference.md) for the full list of supported operators and functions.
- See [Advanced Usage](advanced-usage.md) for join patterns, aggregations, and KQL-specific operators.
