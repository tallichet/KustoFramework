# Operators Reference

Complete reference of all supported KQL operators and functions in KustoFramework.

## Tabular Operators

### Where

Filter rows by a predicate.

```csharp
.Where(e => e.State == "TEXAS")
// | where State == "TEXAS"

.Where(e => e.DamageProperty > 1000 && e.State != "FLORIDA")
// | where DamageProperty > 1000 and State != "FLORIDA"

.Where(e => !(e.State == "TEXAS"))
// | where not(State == "TEXAS")
```

Supported binary operators: `==`, `!=`, `>`, `>=`, `<`, `<=`, `&&` (→ `and`), `||` (→ `or`), `!` (→ `not()`).

Null checks are translated automatically:
```csharp
.Where(e => e.Source == null)   // | where isnull(Source)
.Where(e => e.Source != null)   // | where isnotnull(Source)
```

### Project

Select specific columns, rename, or compute new values.

```csharp
.Project(e => new { e.State, e.EventType })
// | project State, EventType

.Project(e => new { Location = e.State, Type = e.EventType })
// | project Location = State, Type = EventType

.Project(e => new { e.State, TotalCasualties = e.DeathsDirect + e.InjuriesDirect })
// | project State, TotalCasualties = DeathsDirect + InjuriesDirect
```

### ProjectAway

Remove specific columns.

```csharp
.ProjectAway<StormEvent, string>(e => e.Source, e => e.EpisodeNarrative)
// | project-away Source, EpisodeNarrative
```

### Extend

Add computed columns while keeping all original columns.

```csharp
.Extend(e => new { TotalCasualties = e.DeathsDirect + e.InjuriesDirect })
// | extend TotalCasualties = DeathsDirect + InjuriesDirect
```

### OrderBy / OrderByDescending / ThenBy / ThenByDescending

Sort results.

```csharp
.OrderBy(e => e.State)
// | sort by State asc

.OrderByDescending(e => e.DamageProperty)
// | sort by DamageProperty desc

.OrderByDescending(e => e.DamageProperty).ThenBy(e => e.State)
// | sort by DamageProperty desc, State asc
```

### Top

Return the top N rows sorted by a key.

```csharp
.Top(10, e => e.DamageProperty)
// | top 10 by DamageProperty desc

.Top(5, e => e.StartTime, SortOrder.Ascending)
// | top 5 by StartTime asc
```

### Take

Return the first N rows.

```csharp
.Take(100)
// | take 100
```

### Distinct

Return unique rows or unique column combinations.

```csharp
.Distinct()
// | distinct

.Distinct(e => new { e.State, e.EventType })
// | distinct State, EventType
```

### Count

Append a count operator.

```csharp
.Where(e => e.State == "TEXAS").Count()
// | where State == "TEXAS" | count
```

---

## Summarize (Aggregation)

### Without GroupBy

```csharp
.Summarize(e => new { Total = Kql.Count() })
// | summarize Total = count()
```

### With GroupBy

```csharp
.Summarize(
    groupBy: e => e.State,
    aggregation: e => new { Count = Kql.Count(), TotalDamage = Kql.Sum(e.DamageProperty) })
// | summarize Count = count(), TotalDamage = sum(DamageProperty) by State
```

### Multiple GroupBy Keys

```csharp
.Summarize(
    groupBy: e => new { e.State, e.EventType },
    aggregation: e => new { Count = Kql.Count() })
// | summarize Count = count() by State, EventType
```

### Aggregation Functions

| C# | KQL |
|---|---|
| `Kql.Count()` | `count()` |
| `Kql.CountIf(predicate)` | `countif(predicate)` |
| `Kql.Sum(e.Col)` | `sum(Col)` |
| `Kql.SumIf(e.Col, predicate)` | `sumif(Col, predicate)` |
| `Kql.Avg(e.Col)` | `avg(Col)` |
| `Kql.Min(e.Col)` | `min(Col)` |
| `Kql.Max(e.Col)` | `max(Col)` |
| `Kql.DCount(e.Col)` | `dcount(Col)` |
| `Kql.Percentile(e.Col, 95.0)` | `percentile(Col, 95)` |
| `Kql.MakeList(e.Col)` | `make_list(Col)` |
| `Kql.MakeSet(e.Col)` | `make_set(Col)` |
| `Kql.ArgMax(e.Col, e.By)` | `arg_max(Col, By)` |
| `Kql.ArgMin(e.Col, e.By)` | `arg_min(Col, By)` |

---

## Join

```csharp
storms.Join(
    population,
    outerKey: e => e.State,
    innerKey: p => p.State,
    resultSelector: (e, p) => new { e.EventType, p.Population },
    kind: JoinKind.LeftOuter)
// | join kind=leftouter (PopulationData) on State
// | project EventType, Population
```

### Join Kinds

`InnerUnique`, `Inner`, `LeftOuter`, `RightOuter`, `FullOuter`, `LeftSemi`, `LeftAnti`, `RightSemi`, `RightAnti`

## Union

```csharp
storms1.Union(storms2)
// | union StormEvents | where ...

storms1.Union(UnionKind.Outer, storms2)
// | union kind=outer StormEvents | where ...
```

---

## KQL-Specific Operators

### MvExpand

Expand array columns to individual rows.

```csharp
.MvExpand(e => e.Tags)
// | mv-expand Tags
```

### Parse

Extract columns from a string column.

```csharp
.Parse<LogEntry, object>(e => e.RawMessage, "\"Error: \" ErrorMsg:string \" at \" Location:string")
// | parse RawMessage with "Error: " ErrorMsg:string " at " Location:string
```

### BagUnpack

Expand a dynamic column into multiple columns.

```csharp
.BagUnpack<StormEvent, object>(e => e.DynamicBag)
// | evaluate bag_unpack(DynamicBag)
```

### Render

Add a visualization hint.

```csharp
.Render(RenderKind.TimeChart)   // | render timechart
.Render(RenderKind.BarChart)    // | render barchart
.Render(RenderKind.PieChart)    // | render piechart
```

Available: `Table`, `BarChart`, `ColumnChart`, `PieChart`, `TimeChart`, `LineChart`, `AreaChart`, `StackedAreaChart`, `Ladder`, `ScatterChart`, `TreeMap`, `Card`

---

## String Operators

### Standard .NET Methods (auto-translated)

| C# | KQL |
|---|---|
| `.Contains("foo")` | `contains "foo"` |
| `.StartsWith("foo")` | `startswith "foo"` |
| `.EndsWith("foo")` | `endswith "foo"` |
| `.ToLower()` | `tolower(col)` |
| `.ToUpper()` | `toupper(col)` |

### KQL-Specific Extensions

| C# | KQL |
|---|---|
| `.KqlHas("term")` | `has "term"` |
| `.KqlHasCs("term")` | `has_cs "term"` |
| `.KqlHasPrefix("pre")` | `hasprefix "pre"` |
| `.KqlHasSuffix("suf")` | `hassuffix "suf"` |
| `.KqlContains("val")` | `contains "val"` |
| `.KqlMatchesRegex("pattern")` | `matches regex "pattern"` |
| `.KqlIn("a", "b")` | `in ("a", "b")` |
| `.KqlNotIn("a", "b")` | `!in ("a", "b")` |

### Enumerable.Contains → KQL `in`

```csharp
var states = new[] { "TEXAS", "FLORIDA" };
.Where(e => states.Contains(e.State))
// | where State in ("TEXAS", "FLORIDA")
```

### Scalar String Functions

| C# | KQL |
|---|---|
| `Kql.IsEmpty(e.Col)` | `isempty(Col)` |
| `Kql.IsNotEmpty(e.Col)` | `isnotempty(Col)` |
| `Kql.Strlen(e.Col)` | `strlen(Col)` |
| `Kql.Substring(e.Col, 0, 5)` | `substring(Col, 0, 5)` |
| `Kql.Trim("regex", e.Col)` | `trim(regex, Col)` |
| `Kql.ToUpper(e.Col)` | `toupper(Col)` |
| `Kql.ToLower(e.Col)` | `tolower(Col)` |
| `Kql.Strcat(a, b, c)` | `strcat(a, b, c)` |

---

## Time Functions

| C# | KQL |
|---|---|
| `Kql.Ago(TimeSpan.FromDays(7))` | `ago(7d)` |
| `Kql.Ago(TimeSpan.FromHours(4))` | `ago(4h)` |
| `Kql.Ago(TimeSpan.FromMinutes(30))` | `ago(30m)` |
| `Kql.Now()` | `now()` |
| `Kql.Bin(e.Timestamp, TimeSpan.FromHours(1))` | `bin(Timestamp, 1h)` |
| `Kql.StartOfDay(e.Timestamp)` | `startofday(Timestamp)` |
| `Kql.StartOfMonth(e.Timestamp)` | `startofmonth(Timestamp)` |
| `Kql.StartOfWeek(e.Timestamp)` | `startofweek(Timestamp)` |
| `Kql.StartOfYear(e.Timestamp)` | `startofyear(Timestamp)` |

---

## Type Conversion Functions

| C# | KQL |
|---|---|
| `Kql.ToLong(e.Col)` | `tolong(Col)` |
| `Kql.ToInt(e.Col)` | `toint(Col)` |
| `Kql.ToDouble(e.Col)` | `todouble(Col)` |
| `Kql.ToReal(e.Col)` | `toreal(Col)` |
| `Kql.ToString(e.Col)` | `tostring(Col)` |
| `Kql.ToDateTime(e.Col)` | `todatetime(Col)` |
| `Kql.ToTimeSpan(e.Col)` | `totimespan(Col)` |

## Conditional Functions

| C# | KQL |
|---|---|
| `Kql.Iff(predicate, ifTrue, ifFalse)` | `iff(predicate, ifTrue, ifFalse)` |
| `Kql.Coalesce(a, b, c)` | `coalesce(a, b, c)` |
| `Kql.Between(e.Col, from, to)` | `Col between (from .. to)` |
