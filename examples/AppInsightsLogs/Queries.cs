using AppInsightsLogs.Models;
using KustoFramework.Azure;
using KustoFramework.Extensions;
using KustoFramework.Functions;
using KustoFramework.Query;

namespace AppInsightsLogs;

public static class Queries
{
    public static KqlQuery<AppInsightsTrace> RecentTraces(KustoClient client, TimeSpan lookback, int top, int? minSeverity = null)
    {
        var query = client.Table<AppInsightsTrace>()
            .Where(t => t.Timestamp > Kql.Ago(lookback));

        if (minSeverity.HasValue)
        {
            var severity = minSeverity.Value;
            query = query.Where(t => t.SeverityLevel >= severity);
        }

        return query
            .OrderByDescending(t => t.Timestamp)
            .Take(top);
    }

    public static KqlQuery<AppInsightsRequest> SlowRequests(KustoClient client, TimeSpan lookback, int top)
    {
        return client.Table<AppInsightsRequest>()
            .Where(r => r.Timestamp > Kql.Ago(lookback))
            .OrderByDescending(r => r.Duration)
            .Take(top);
    }

    public static KqlQuery<AppInsightsException> RecentExceptions(KustoClient client, TimeSpan lookback, int top)
    {
        return client.Table<AppInsightsException>()
            .Where(e => e.Timestamp > Kql.Ago(lookback))
            .OrderByDescending(e => e.Timestamp)
            .Take(top);
    }

    public static KqlQuery<AppInsightsDependency> FailedDependencies(KustoClient client, TimeSpan lookback, int top)
    {
        return client.Table<AppInsightsDependency>()
            .Where(d => d.Timestamp > Kql.Ago(lookback) && d.Success == false)
            .OrderByDescending(d => d.Timestamp)
            .Take(top);
    }
}
