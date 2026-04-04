namespace KustoFramework.Attributes;

/// <summary>
/// Specifies the KQL table name for a model class when it differs from the C# type name.
/// </summary>
/// <example>
/// <code>
/// [KqlTable("StormEvents")]
/// public class StormEvent { ... }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class KqlTableAttribute(string name) : Attribute
{
    /// <summary>The KQL table name.</summary>
    public string Name { get; } = name;
}
