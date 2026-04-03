namespace KustoFramework.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class KqlTableAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
