# Stop Writing Raw KQL Strings in C#: Introducing KustoFramework

*A type-safe, LINQ-inspired query builder for Kusto Query Language*

---

## The Problem With Raw KQL Strings

If you query Azure Data Explorer (or Azure Monitor / Log Analytics) from a .NET application, you have probably written something like this at some point:

```csharp
// Somewhere in a service class...
string state = GetStateFromRequest();
int days = 7;

string kql = $@"
    StormEvents
    | where State == ""{state}""
    | where StartTime > ago({days}d)
    | project StartTime, EventType, DmgProperty
    | top 20 by DmgProperty desc
";

var results = await kustoClient.ExecuteQueryAsync(database, kql, null);
```

It works. Until it doesn't.

A week later a colleague renames `DmgProperty` to `DamageProperty` in the data model. She updates the C# DTO. The compiler raises zero warnings. The code ships. At runtime every query silently returns no rows because the column name is wrong — or worse, throws a Kusto `SemanticError` that only surfaces in production logs at 2 AM.

This is the fundamental problem with embedding KQL as plain strings in C# code:

- **No IntelliSense** — you are typing blind inside a string literal.
- **No compile-time safety** — column renames, typos, and structural mistakes are invisible until runtime.
- **Not refactor-friendly** — a global rename in your C# model will not update your query strings.
- **Hard to test** — asserting the shape of a raw string is brittle and doesn't scale.
- **No reuse** — copy-pasting query fragments across the codebase leads to drift and duplication.

There is a better way.

---

## Introducing KustoFramework

[KustoFramework](https://www.nuget.org/packages/KustoFramework) is a strongly-typed, LINQ-inspired query builder for KQL, targeting .NET 10. It lets you write KQL queries using C# expressions — with full IntelliSense, compile-time validation, and zero runtime execution overhead.

It does **not** require the Kusto SDK to build queries. It is a pure .NET library with no dependencies. You write C#, it gives you back a KQL string that you can pass to any client you already use.

Install it with:

```bash
dotnet add package KustoFramework
```

---

## Getting Started

### Step 1 — Map your table to a C# class

Define a plain C# class that represents each row in your Kusto table. Add `[KqlTable]` if your C# class name differs from the actual table name, and `[KqlColumn]` for individual column overrides:

```csharp
using KustoFramework.Attributes;

[KqlTable("StormEvents")]   // maps to the "StormEvents" table in Kusto
public class StormEvent
{
    public DateTime StartTime { get; set; }
    public string State { get; set; }
    public string EventType { get; set; }
    public int DamageProperty { get; set; }
    public int DeathsDirect { get; set; }
    public int InjuriesDirect { get; set; }
}
```

If the class name already matches the table name, the attribute is optional.

### Step 2 — Write your first query

```csharp
using KustoFramework;
using KustoFramework.Functions;

var ctx = new KustoContext();

string kql = ctx.Table<StormEvent>()
    .Where(e => e.State == "TEXAS" && e.StartTime > Kql.Ago(TimeSpan.FromDays(7)))
    .Project(e => new { e.StartTime, e.EventType, e.DamageProperty })
    .ToKql();
```

`ToKql()` returns a ready-to-execute KQL string:

```kql
StormEvents
| where State == "TEXAS" and StartTime > ago(7d)
| project StartTime, EventType, DamageProperty
```

No string interpolation. No raw text. If you rename `DamageProperty` in your C# class, the compiler will tell you every query that references it needs updating — before you deploy.

---

## Core Operators in Action

### filter, compute, sort, paginate

The familiar pipeline operators map directly to their KQL equivalents:

```csharp
string kql = ctx.Table<StormEvent>()
    .Where(e => e.DamageProperty > 0)
    .Extend(e => new { TotalCasualties = e.DeathsDirect + e.InjuriesDirect })
    .OrderByDescending(e => e.TotalCasualties)
    .Take(50)
    .ToKql();
```

```kql
StormEvents
| where DamageProperty > 0
| extend TotalCasualties = DeathsDirect + InjuriesDirect
| sort by TotalCasualties desc
| take 50
```

### Aggregation with summarize

`Summarize` accepts a `groupBy` expression and an `aggregation` expression. You can use any of the built-in KQL aggregation functions via the `Kql` static class:

```csharp
string kql = ctx.Table<StormEvent>()
    .Summarize(
        groupBy: e => e.State,
        aggregation: e => new
        {
            Count      = Kql.Count(),
            TotalDamage = Kql.Sum(e.DamageProperty)
        })
    .Top(10, x => x.TotalDamage)
    .ToKql();
```

```kql
StormEvents
| summarize Count = count(), TotalDamage = sum(DamageProperty) by State
| top 10 by TotalDamage desc
```

Available aggregation functions include `Count()`, `CountIf()`, `Sum()`, `SumIf()`, `Avg()`, `Min()`, `Max()`, `DCount()`, `Percentile()`, `MakeList()`, `MakeSet()`, `ArgMax()`, and `ArgMin()`.

### Counting rows

```csharp
string kql = ctx.Table<StormEvent>()
    .Where(e => e.State == "TEXAS")
    .Where(e => e.EventType == "Tornado")
    .Count()
    .ToKql();
```

```kql
StormEvents
| where State == "TEXAS"
| where EventType == "Tornado"
| count
```

### Deduplication

```csharp
string kql = ctx.Table<StormEvent>()
    .Where(e => e.DamageProperty > 0)
    .Distinct(e => new { e.State, e.EventType })
    .ToKql();
```

```kql
StormEvents
| where DamageProperty > 0
| distinct State, EventType
```

---

## Advanced Patterns

### Immutable query fragments — write once, branch many times

Every operator in KustoFramework returns a **new** query object. The original is never mutated. This lets you define a common filter once and derive multiple specialized queries from it:

```csharp
// Base fragment — reused across the application
var baseQuery = ctx.Table<StormEvent>()
    .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromDays(30)));

// Branch 1 — quick scalar count
string countKql    = baseQuery.Count().ToKql();

// Branch 2 — top 5 costliest events
string topDamage   = baseQuery.Top(5, e => e.DamageProperty).ToKql();
```

Both branches produce independent KQL strings. Storing `baseQuery` as a field in a service class and composing on top of it is a safe pattern — no hidden shared state.

This immutability is especially useful in ASP.NET services where a base scoped query (e.g., filtered by tenant, time range, or environment) is constructed once and individual endpoints simply append their own operators before calling `ToKql()`:

```csharp
public class StormQueryService
{
    private readonly KqlQuery<StormEvent> _scopedBase;

    public StormQueryService(KustoContext ctx, string region)
    {
        // Base query shared by all methods in this service
        _scopedBase = ctx.Table<StormEvent>()
            .Where(e => e.State == region)
            .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromDays(90)));
    }

    public string GetTopDamageQuery(int n) =>
        _scopedBase.Top(n, e => e.DamageProperty).ToKql();

    public string GetEventCountByTypeQuery() =>
        _scopedBase
            .Summarize(
                groupBy: e => e.EventType,
                aggregation: e => new { Count = Kql.Count() })
            .OrderByDescending(e => e.Count)
            .ToKql();
}
```

Each call site gets a fresh, fully independent KQL string without duplicating the scoping filter.

### Time-series charts in two lines

KQL's `bin()` + `render` combination is a first-class citizen:

```csharp
string kql = ctx.Table<StormEvent>()
    .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromDays(30)))
    .Summarize(
        groupBy: e => Kql.Bin(e.StartTime, TimeSpan.FromDays(1)),
        aggregation: e => new { Count = Kql.Count() })
    .Render(RenderKind.TimeChart)
    .ToKql();
```

```kql
StormEvents
| where StartTime > ago(30d)
| summarize Count = count() by bin(StartTime, 1d)
| render timechart
```

Available render types include `TimeChart`, `BarChart`, `ColumnChart`, `PieChart`, `LineChart`, `AreaChart`, `TreeMap`, `ScatterChart`, and more.

### Joining tables

`Join` supports all nine KQL join kinds (`InnerUnique`, `Inner`, `LeftOuter`, `RightOuter`, `FullOuter`, `LeftSemi`, `LeftAnti`, `RightSemi`, `RightAnti`). The inner table can itself be a full query pipeline:

```csharp
var storms     = ctx.Table<StormEvent>().Where(e => e.DamageProperty > 0);
var population = ctx.Table<PopulationData>();

string kql = storms
    .Join(
        population,
        outerKey:       e => e.State,
        innerKey:       p => p.State,
        resultSelector: (e, p) => new { e.State, e.EventType, p.Population },
        kind: JoinKind.LeftOuter)
    .ToKql();
```

```kql
StormEvents
| where DamageProperty > 0
| join kind=leftouter (PopulationData) on State
| project State, EventType, Population
```

### Captured variables and arrays

C# variables and collections captured in lambda expressions are serialized at `ToKql()` call time:

```csharp
var threshold = 1_000_000;
var states    = new[] { "TEXAS", "FLORIDA", "CALIFORNIA" };

string kql = ctx.Table<StormEvent>()
    .Where(e => e.DamageProperty > threshold)
    .Where(e => states.Contains(e.State))
    .ToKql();
```

```kql
StormEvents
| where DamageProperty > 1000000
| where State in ("TEXAS", "FLORIDA", "CALIFORNIA")
```

`IEnumerable.Contains()` on a C# collection is automatically translated to KQL's `in` operator.

### Expanding array columns with mv-expand

Kusto tables often store arrays or dynamic values in a single column. `MvExpand` unrolls each element into its own row, letting you filter or aggregate on array contents:

```csharp
string kql = ctx.Table<StormEvent>()
    .MvExpand(e => e.Tags)
    .Where(e => e.State == "TEXAS")
    .Project(e => new { e.State, e.Tags })
    .ToKql();
```

```kql
StormEvents
| mv-expand Tags
| where State == "TEXAS"
| project State, Tags
```

---

## A Note on String Operator Performance

When filtering string columns in Kusto, not all operators are equal. The inverted index is only leveraged by term-based operators. Here is the hierarchy from fastest to slowest:

| Operator | KQL | Index? |
|---|---|---|
| `e.Col.KqlHas("term")` | `Col has "term"` | ✅ Full inverted index |
| `e.Col.KqlHasPrefix("pre")` | `Col hasprefix "pre"` | ✅ Partial |
| `e.Col.StartsWith("pre")` | `Col startswith "pre"` | ✅ Partial |
| `e.Col.Contains("str")` | `Col contains "str"` | ❌ Full scan |
| `e.Col.KqlMatchesRegex("p")` | `Col matches regex "p"` | ❌ Full scan + regex |

**Rule of thumb:** use `KqlHas()` whenever you are searching for a complete word. Reserve `Contains()` and regex only when you genuinely need substring or pattern matching.

```csharp
// Prefer this (uses inverted index)
.Where(e => e.Source.KqlHas("newspaper"))

// Over this (full column scan)
.Where(e => e.Source.Contains("newspaper"))
```

---

## Summary

KustoFramework brings the same compile-time discipline to KQL queries that LINQ brought to SQL. Column renames become compiler errors instead of runtime surprises. Query fragments become reusable, testable objects instead of copy-pasted strings. And you keep your existing Kusto client — KustoFramework only builds the query string; execution is up to you.

**Get started:**

```bash
dotnet add package KustoFramework
```

- **NuGet:** https://www.nuget.org/packages/KustoFramework
- **GitHub:** https://github.com/tallichet/KustoFramework
- **Docs:** [Getting Started](getting-started.md) · [Operators Reference](operators-reference.md) · [Advanced Usage](advanced-usage.md)

Feedback, bug reports, and pull requests are welcome. If KustoFramework solves a real pain for your team, leave a ⭐ on GitHub — it helps others discover it.

---

*Published on Medium · April 2026*
