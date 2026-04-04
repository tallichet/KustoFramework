using KustoFramework.Attributes;

namespace AppInsightsLogs.Models;

[KqlTable("exceptions")]
public class AppInsightsException
{
    [KqlColumn("timestamp")]
    public DateTime Timestamp { get; set; }

    [KqlColumn("type")]
    public string Type { get; set; } = "";

    [KqlColumn("message")]
    public string Message { get; set; } = "";

    [KqlColumn("outerMessage")]
    public string OuterMessage { get; set; } = "";

    [KqlColumn("operation_Id")]
    public string OperationId { get; set; } = "";

    [KqlColumn("cloud_RoleName")]
    public string CloudRoleName { get; set; } = "";

    [KqlColumn("details")]
    public string Details { get; set; } = "";
}
