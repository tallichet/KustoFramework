namespace KustoFramework.Attributes;

/// <summary>
/// Specifies the KQL column name for a property when it differs from the C# property name.
/// </summary>
/// <example>
/// <code>
/// [KqlColumn("StartTime")]
/// public DateTime Start { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class KqlColumnAttribute(string name) : Attribute
{
    /// <summary>The KQL column name.</summary>
    public string Name { get; } = name;
}
