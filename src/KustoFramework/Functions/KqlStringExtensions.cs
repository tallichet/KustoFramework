namespace KustoFramework.Functions;

/// <summary>
/// KQL-specific string operators as extension methods.
/// These are marker methods — intercepted by the expression visitor and translated to KQL syntax.
/// </summary>
public static class KqlStringExtensions
{
    /// <summary>Tests if the string contains the whole <paramref name="term"/> as a discrete token. KQL: <c>has</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="term">The term to search for (case-insensitive).</param>
    public static bool KqlHas(this string source, string term) => throw new InvalidOperationException("Marker method.");

    /// <summary>Case-sensitive version of <see cref="KqlHas"/>. KQL: <c>has_cs</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="term">The term to search for (case-sensitive).</param>
    public static bool KqlHasCs(this string source, string term) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string starts with the given <paramref name="prefix"/> as a term. KQL: <c>hasprefix</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="prefix">The prefix to match (case-insensitive).</param>
    public static bool KqlHasPrefix(this string source, string prefix) => throw new InvalidOperationException("Marker method.");

    /// <summary>Case-sensitive version of <see cref="KqlHasPrefix"/>. KQL: <c>hasprefix_cs</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="prefix">The prefix to match (case-sensitive).</param>
    public static bool KqlHasPrefixCs(this string source, string prefix) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string ends with the given <paramref name="suffix"/> as a term. KQL: <c>hassuffix</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="suffix">The suffix to match (case-insensitive).</param>
    public static bool KqlHasSuffix(this string source, string suffix) => throw new InvalidOperationException("Marker method.");

    /// <summary>Case-sensitive version of <see cref="KqlHasSuffix"/>. KQL: <c>hassuffix_cs</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="suffix">The suffix to match (case-sensitive).</param>
    public static bool KqlHasSuffixCs(this string source, string suffix) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string contains the given <paramref name="value"/> as a substring. KQL: <c>contains</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="value">The substring to search for (case-insensitive).</param>
    public static bool KqlContains(this string source, string value) => throw new InvalidOperationException("Marker method.");

    /// <summary>Case-sensitive version of <see cref="KqlContains"/>. KQL: <c>contains_cs</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="value">The substring to search for (case-sensitive).</param>
    public static bool KqlContainsCs(this string source, string value) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string starts with the given <paramref name="value"/>. KQL: <c>startswith</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="value">The prefix to match (case-insensitive).</param>
    public static bool KqlStartsWith(this string source, string value) => throw new InvalidOperationException("Marker method.");

    /// <summary>Case-sensitive version of <see cref="KqlStartsWith"/>. KQL: <c>startswith_cs</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="value">The prefix to match (case-sensitive).</param>
    public static bool KqlStartsWithCs(this string source, string value) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string ends with the given <paramref name="value"/>. KQL: <c>endswith</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="value">The suffix to match (case-insensitive).</param>
    public static bool KqlEndsWith(this string source, string value) => throw new InvalidOperationException("Marker method.");

    /// <summary>Case-sensitive version of <see cref="KqlEndsWith"/>. KQL: <c>endswith_cs</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="value">The suffix to match (case-sensitive).</param>
    public static bool KqlEndsWithCs(this string source, string value) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string matches the given regular expression <paramref name="pattern"/>. KQL: <c>matches regex</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="pattern">The regex pattern.</param>
    public static bool KqlMatchesRegex(this string source, string pattern) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string equals any of the given <paramref name="values"/>. KQL: <c>in (...)</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="values">The values to match against.</param>
    public static bool KqlIn(this string source, params string[] values) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string does not equal any of the given <paramref name="values"/>. KQL: <c>!in (...)</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="values">The values to exclude.</param>
    public static bool KqlNotIn(this string source, params string[] values) => throw new InvalidOperationException("Marker method.");

    // ─── Negated string operators ────────────────────────────────────────

    /// <summary>Tests if the string does not contain the <paramref name="term"/> as a discrete token. KQL: <c>!has</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="term">The term to search for (case-insensitive).</param>
    public static bool KqlNotHas(this string source, string term) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string does not contain the given <paramref name="value"/> as a substring. KQL: <c>!contains</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="value">The substring to search for (case-insensitive).</param>
    public static bool KqlNotContains(this string source, string value) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string does not start with the given <paramref name="value"/>. KQL: <c>!startswith</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="value">The prefix to check (case-insensitive).</param>
    public static bool KqlNotStartsWith(this string source, string value) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string does not end with the given <paramref name="value"/>. KQL: <c>!endswith</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="value">The suffix to check (case-insensitive).</param>
    public static bool KqlNotEndsWith(this string source, string value) => throw new InvalidOperationException("Marker method.");

    // ─── Multi-value string operators ────────────────────────────────────

    /// <summary>Tests if the string contains any of the given <paramref name="values"/> as terms. KQL: <c>has_any(...)</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="values">The terms to match against.</param>
    public static bool KqlHasAny(this string source, params string[] values) => throw new InvalidOperationException("Marker method.");

    /// <summary>Tests if the string contains all of the given <paramref name="values"/> as terms. KQL: <c>has_all(...)</c>.</summary>
    /// <param name="source">The string column.</param>
    /// <param name="values">The terms that must all be present.</param>
    public static bool KqlHasAll(this string source, params string[] values) => throw new InvalidOperationException("Marker method.");
}
