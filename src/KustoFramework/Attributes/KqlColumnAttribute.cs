namespace KustoFramework.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class KqlColumnAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
