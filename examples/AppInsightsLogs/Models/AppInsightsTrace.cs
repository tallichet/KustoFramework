using KustoFramework.Attributes;

namespace AppInsightsLogs.Models;

[KqlTable("traces")]
public class AppInsightsTrace
{
    [KqlColumn("timestamp")]
    public DateTime Timestamp { get; set; }

    [KqlColumn("message")]
    public string Message { get; set; } = "";

    [KqlColumn("severityLevel")]
    public int SeverityLevel { get; set; }

    [KqlColumn("operation_Id")]
    public string OperationId { get; set; } = "";

    [KqlColumn("operation_Name")]
    public string OperationName { get; set; } = "";

    [KqlColumn("cloud_RoleName")]
    public string CloudRoleName { get; set; } = "";

    [KqlColumn("customDimensions")]
    public string CustomDimensions { get; set; } = "";
}
