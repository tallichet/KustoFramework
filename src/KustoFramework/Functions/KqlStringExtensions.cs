namespace KustoFramework.Functions;

/// <summary>
/// KQL-specific string operators as extension methods.
/// These are marker methods — intercepted by the expression visitor.
/// </summary>
public static class KqlStringExtensions
{
    public static bool KqlHas(this string source, string term) => throw new InvalidOperationException("Marker method.");
    public static bool KqlHasCs(this string source, string term) => throw new InvalidOperationException("Marker method.");
    public static bool KqlHasPrefix(this string source, string prefix) => throw new InvalidOperationException("Marker method.");
    public static bool KqlHasPrefixCs(this string source, string prefix) => throw new InvalidOperationException("Marker method.");
    public static bool KqlHasSuffix(this string source, string suffix) => throw new InvalidOperationException("Marker method.");
    public static bool KqlHasSuffixCs(this string source, string suffix) => throw new InvalidOperationException("Marker method.");
    public static bool KqlContains(this string source, string value) => throw new InvalidOperationException("Marker method.");
    public static bool KqlContainsCs(this string source, string value) => throw new InvalidOperationException("Marker method.");
    public static bool KqlStartsWith(this string source, string value) => throw new InvalidOperationException("Marker method.");
    public static bool KqlStartsWithCs(this string source, string value) => throw new InvalidOperationException("Marker method.");
    public static bool KqlEndsWith(this string source, string value) => throw new InvalidOperationException("Marker method.");
    public static bool KqlEndsWithCs(this string source, string value) => throw new InvalidOperationException("Marker method.");
    public static bool KqlMatchesRegex(this string source, string pattern) => throw new InvalidOperationException("Marker method.");
    public static bool KqlIn(this string source, params string[] values) => throw new InvalidOperationException("Marker method.");
    public static bool KqlNotIn(this string source, params string[] values) => throw new InvalidOperationException("Marker method.");
}
