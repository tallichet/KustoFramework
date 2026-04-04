namespace KustoFramework.Enums;

/// <summary>Specifies the union kind for the KQL <c>union</c> operator.</summary>
public enum UnionKind
{
    /// <summary>KQL: <c>kind=inner</c> — only columns common to all tables.</summary>
    Inner,
    /// <summary>KQL: <c>kind=outer</c> — all columns from all tables (missing values are null).</summary>
    Outer
}
