# App Insights Log Viewer

A minimal CLI that queries Application Insights data from a Log Analytics workspace using **KustoFramework** and **KustoFramework.Azure**.

This example demonstrates:

- Defining models with `[KqlTable]` and `[KqlColumn]` attributes for App Insights tables
- Building typed KQL queries using the LINQ-like fluent API
- Executing queries against a Log Analytics workspace via `KustoClient`
- Using the `--dry-run` flag to inspect generated KQL without executing

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Azure CLI installed and signed in (`az login`)
- An Application Insights resource connected to a **Log Analytics workspace**

## Configuration

Set the following environment variables:

```bash
export APP_INSIGHTS_CLUSTER_URI="https://ade.loganalytics.io"
export APP_INSIGHTS_DATABASE="/subscriptions/<sub-id>/resourcegroups/<rg>/providers/microsoft.operationalinsights/workspaces/<workspace-name>"
```

Or edit `appsettings.json` with the same values.

### Finding your workspace resource path

1. Go to the [Azure portal](https://portal.azure.com) → your **Log Analytics workspace**
2. In the **Overview** blade, note the **Subscription ID**, **Resource group**, and **Workspace name**
3. The database value is: `/subscriptions/{sub}/resourcegroups/{rg}/providers/microsoft.operationalinsights/workspaces/{name}`

## Usage

```bash
# From the repository root:
dotnet run --project examples/AppInsightsLogs -- <command> [options]
```

### Commands

| Command | Description |
|---|---|
| `traces` | Recent application traces, sorted by time |
| `requests` | Slowest HTTP requests |
| `exceptions` | Recent exceptions |
| `dependencies` | Failed dependency calls |

### Options

| Option | Description | Default |
|---|---|---|
| `--last <duration>` | Lookback window (`30m`, `1h`, `24h`, `7d`) | `1h` |
| `--top <N>` | Maximum number of rows | `20` |
| `--severity <level>` | Minimum severity level, `traces` only (0–4) | all |
| `--dry-run` | Print the generated KQL without executing | off |
| `--help` | Show help | — |

### Examples

```bash
# Show the last hour of traces (default)
dotnet run --project examples/AppInsightsLogs -- traces

# Show warnings and errors from the last 24 hours
dotnet run --project examples/AppInsightsLogs -- traces --last 24h --severity 2

# Show the 10 slowest requests from the last 7 days
dotnet run --project examples/AppInsightsLogs -- requests --last 7d --top 10

# Show the generated KQL for failed dependencies without executing
dotnet run --project examples/AppInsightsLogs -- dependencies --last 1h --dry-run
```

The `--dry-run` output looks like:

```
// Generated KQL:
dependencies
| where timestamp > ago(1h) and success == false
| sort by timestamp desc
| take 20
```
