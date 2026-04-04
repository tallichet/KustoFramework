using System.Text.Json;
using AppInsightsLogs;
using AppInsightsLogs.Models;
using KustoFramework.Azure;
using KustoFramework.Azure.Extensions;

// ---------- Parse arguments ----------

if (args.Length == 0 || args[0] is "--help" or "-h")
{
    PrintUsage();
    return 0;
}

var command = args[0];
var lookback = TimeSpan.FromHours(1);
var top = 20;
var dryRun = false;
int? minSeverity = null;

for (int i = 1; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--last" when i + 1 < args.Length:
            lookback = ParseDuration(args[++i]);
            break;
        case "--top" when i + 1 < args.Length:
            top = int.Parse(args[++i]);
            break;
        case "--severity" when i + 1 < args.Length:
            minSeverity = int.Parse(args[++i]);
            break;
        case "--dry-run":
            dryRun = true;
            break;
        default:
            Console.Error.WriteLine($"Unknown argument: {args[i]}");
            PrintUsage();
            return 1;
    }
}

// ---------- Load configuration ----------

var clusterUri = Environment.GetEnvironmentVariable("APP_INSIGHTS_CLUSTER_URI");
var database = Environment.GetEnvironmentVariable("APP_INSIGHTS_DATABASE");

if (string.IsNullOrEmpty(clusterUri) || string.IsNullOrEmpty(database))
{
    var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    if (File.Exists(settingsPath))
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(settingsPath));
        clusterUri ??= doc.RootElement.GetProperty("ClusterUri").GetString();
        database ??= doc.RootElement.GetProperty("Database").GetString();
    }
}

if (string.IsNullOrWhiteSpace(clusterUri) || string.IsNullOrWhiteSpace(database))
{
    Console.Error.WriteLine("Error: Set APP_INSIGHTS_CLUSTER_URI and APP_INSIGHTS_DATABASE environment variables, or configure appsettings.json.");
    return 1;
}

// ---------- Build client ----------

using var client = new KustoClient(new KustoConnectionOptions
{
    ClusterUri = clusterUri,
    Database = database,
    ConfigureConnection = kcsb => kcsb.WithAadAzCliAuthentication()
});

// ---------- Execute command ----------

try
{
    switch (command)
    {
        case "traces":
            var traces = Queries.RecentTraces(client, lookback, top, minSeverity);
            if (dryRun) { PrintKql(traces.ToKql()); return 0; }
            PrintKql(traces.ToKql());
            PrintTable(
                ["Timestamp", "Severity", "Role", "Message"],
                (await traces.ToListAsync(client)).Select(t => new[]
                {
                    t.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    t.SeverityLevel.ToString(),
                    t.CloudRoleName,
                    Truncate(t.Message, 80)
                }));
            break;

        case "requests":
            var requests = Queries.SlowRequests(client, lookback, top);
            if (dryRun) { PrintKql(requests.ToKql()); return 0; }
            PrintKql(requests.ToKql());
            PrintTable(
                ["Timestamp", "Duration", "Code", "Role", "Name"],
                (await requests.ToListAsync(client)).Select(r => new[]
                {
                    r.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    r.Duration.TotalMilliseconds.ToString("F0") + "ms",
                    r.ResultCode,
                    r.CloudRoleName,
                    Truncate(r.Name, 60)
                }));
            break;

        case "exceptions":
            var exceptions = Queries.RecentExceptions(client, lookback, top);
            if (dryRun) { PrintKql(exceptions.ToKql()); return 0; }
            PrintKql(exceptions.ToKql());
            PrintTable(
                ["Timestamp", "Role", "Type", "Message"],
                (await exceptions.ToListAsync(client)).Select(e => new[]
                {
                    e.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    e.CloudRoleName,
                    e.Type,
                    Truncate(e.Message, 60)
                }));
            break;

        case "dependencies":
            var deps = Queries.FailedDependencies(client, lookback, top);
            if (dryRun) { PrintKql(deps.ToKql()); return 0; }
            PrintKql(deps.ToKql());
            PrintTable(
                ["Timestamp", "Duration", "Type", "Target", "Name"],
                (await deps.ToListAsync(client)).Select(d => new[]
                {
                    d.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    d.Duration.TotalMilliseconds.ToString("F0") + "ms",
                    d.Type,
                    Truncate(d.Target, 30),
                    Truncate(d.Name, 50)
                }));
            break;

        default:
            Console.Error.WriteLine($"Unknown command: {command}");
            PrintUsage();
            return 1;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

return 0;

// ---------- Helpers ----------

static void PrintUsage()
{
    Console.WriteLine("""
        App Insights Log Viewer — KustoFramework example

        Usage: AppInsightsLogs <command> [options]

        Commands:
          traces         Recent application traces
          requests       Slowest HTTP requests
          exceptions     Recent exceptions
          dependencies   Failed dependency calls

        Options:
          --last <duration>   Lookback window (e.g. 1h, 24h, 7d). Default: 1h
          --top <N>           Max rows to return. Default: 20
          --severity <level>  Min severity level, traces only (0=Verbose .. 4=Critical)
          --dry-run           Print the generated KQL query without executing
          --help, -h          Show this help
        """);
}

static void PrintKql(string kql)
{
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"\n// Generated KQL:\n{kql}\n");
    Console.ResetColor();
}

static void PrintTable(string[] headers, IEnumerable<string[]> rows)
{
    var allRows = rows.ToList();
    if (allRows.Count == 0)
    {
        Console.WriteLine("(no results)");
        return;
    }

    var widths = new int[headers.Length];
    for (int i = 0; i < headers.Length; i++)
        widths[i] = headers[i].Length;

    foreach (var row in allRows)
        for (int i = 0; i < Math.Min(row.Length, widths.Length); i++)
            widths[i] = Math.Max(widths[i], (row[i] ?? "").Length);

    var format = string.Join("  ", widths.Select((w, i) => $"{{{i},-{w}}}"));

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(string.Format(format, headers));
    Console.WriteLine(new string('─', widths.Sum() + (widths.Length - 1) * 2));
    Console.ResetColor();

    foreach (var row in allRows)
    {
        var padded = new string[headers.Length];
        for (int i = 0; i < headers.Length; i++)
            padded[i] = i < row.Length ? (row[i] ?? "") : "";
        Console.WriteLine(string.Format(format, padded));
    }

    Console.WriteLine($"\n{allRows.Count} row(s)");
}

static TimeSpan ParseDuration(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return TimeSpan.FromHours(1);

    var span = input.AsSpan().Trim();
    var suffix = span[^1];
    if (double.TryParse(span[..^1], out var value))
    {
        return suffix switch
        {
            'm' => TimeSpan.FromMinutes(value),
            'h' => TimeSpan.FromHours(value),
            'd' => TimeSpan.FromDays(value),
            _ => throw new ArgumentException($"Unknown duration suffix '{suffix}'. Use m, h, or d.")
        };
    }

    throw new ArgumentException($"Invalid duration: {input}. Examples: 30m, 1h, 7d");
}

static string Truncate(string value, int maxLength) =>
    string.IsNullOrEmpty(value) ? "" :
    value.Length <= maxLength ? value :
    string.Concat(value.AsSpan(0, maxLength - 1), "…");
