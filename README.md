# KustoFramework

[![Build](https://github.com/tallichet/KustoFramework/actions/workflows/ci.yml/badge.svg)](https://github.com/tallichet/KustoFramework/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/KustoFramework.svg)](https://www.nuget.org/packages/KustoFramework)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A strongly-typed, LINQ-inspired query builder for [Kusto Query Language (KQL)](https://learn.microsoft.com/en-us/azure/data-explorer/kusto/query/), targeting .NET 10.

KustoFramework lets you build KQL queries using C# expressions — with full IntelliSense, compile-time safety, and zero runtime execution overhead.

```csharp
var kql = new KustoContext()
    .Table<StormEvent>()
    .Where(e => e.State == "TEXAS" && e.StartTime > Kql.Ago(TimeSpan.FromDays(7)))
    .Project(e => new { e.StartTime, e.EventType, e.DamageProperty })
    .ToKql();
```

Produces:

```kql
StormEvents
| where State == "TEXAS" and StartTime > ago(7d)
| project StartTime, EventType, DamageProperty
```

## Features

- **Strongly-typed** — queries are based on your C# model classes; renaming a property updates your queries at compile time
- **Immutable query pipeline** — each operator returns a new query, making fragments reusable and safe to share
- **Rich operator support** — `where`, `project`, `project-away`, `extend`, `summarize`, `order by`, `top`, `take`, `distinct`, `count`, `join`, `union`, `mv-expand`, `render`
- **KQL function library** — aggregations (`count()`, `sum()`, `avg()`, `dcount()`, …), time functions (`ago()`, `bin()`, `startofday()`, …), string functions, type conversions, and more
- **Zero dependencies** — pure .NET, no Kusto SDK required to build queries

## Installation

```bash
dotnet add package KustoFramework
```

> Requires .NET 10 or later.

## Quick Start

### 1. Define your model

```csharp
using KustoFramework.Attributes;

[KqlTable("StormEvents")]   // optional: override the table name
public class StormEvent
{
    public string State { get; set; }
    public string EventType { get; set; }
    public DateTime StartTime { get; set; }
    public long DamageProperty { get; set; }
    public int DeathsDirect { get; set; }
    public int InjuriesDirect { get; set; }
}
```

> If `[KqlTable]` is omitted, the class name is used as the table name.

### 2. Build queries

```csharp
using KustoFramework;
using KustoFramework.Extensions;
using KustoFramework.Functions;

var ctx = new KustoContext();

// Simple filter + projection
string kql = ctx.Table<StormEvent>()
    .Where(e => e.DamageProperty > 0)
    .Project(e => new { e.State, e.EventType, e.DamageProperty })
    .ToKql();

// Aggregation
string summary = ctx.Table<StormEvent>()
    .Summarize(
        groupBy: e => e.State,
        aggregation: e => new { Count = Kql.Count(), TotalDamage = Kql.Sum(e.DamageProperty) })
    .Top(10, x => x.TotalDamage)
    .ToKql();

// Reusable fragments — queries are immutable
var baseQuery = ctx.Table<StormEvent>()
    .Where(e => e.StartTime > Kql.Ago(TimeSpan.FromDays(30)));

string countKql  = baseQuery.Count().ToKql();
string topDamage = baseQuery.Top(5, e => e.DamageProperty).ToKql();
```

## Documentation

- [Getting Started](docs/getting-started.md)
- [Operators Reference](docs/operators-reference.md)
- [Advanced Usage](docs/advanced-usage.md)

## Examples

- [App Insights Log Viewer](examples/AppInsightsLogs/) — A minimal CLI that queries Application Insights data from a Log Analytics workspace, demonstrating model definition, typed query building, and query execution with `KustoFramework.Azure`.

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request.

## License

This project is licensed under the [MIT License](LICENSE).
