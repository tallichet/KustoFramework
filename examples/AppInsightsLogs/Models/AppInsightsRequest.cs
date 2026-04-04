using KustoFramework.Attributes;

namespace AppInsightsLogs.Models;

[KqlTable("requests")]
public class AppInsightsRequest
{
    [KqlColumn("timestamp")]
    public DateTime Timestamp { get; set; }

    [KqlColumn("name")]
    public string Name { get; set; } = "";

    [KqlColumn("url")]
    public string Url { get; set; } = "";

    [KqlColumn("duration")]
    public TimeSpan Duration { get; set; }

    [KqlColumn("resultCode")]
    public string ResultCode { get; set; } = "";

    [KqlColumn("success")]
    public bool Success { get; set; }

    [KqlColumn("operation_Id")]
    public string OperationId { get; set; } = "";

    [KqlColumn("cloud_RoleName")]
    public string CloudRoleName { get; set; } = "";
}
