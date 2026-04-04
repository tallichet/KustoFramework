using System.Linq.Expressions;

namespace KustoFramework.Functions;

/// <summary>
/// Static marker methods for KQL functions. These methods are never executed at runtime —
/// they are intercepted by the expression visitor and translated to KQL syntax.
/// </summary>
public static class Kql
{
    // ─── Aggregation functions ───────────────────────────────────────────

    /// <summary>Counts the number of records. KQL: <c>count()</c>.</summary>
    /// <returns>The record count.</returns>
    public static long Count() => throw new InvalidOperationException("Kql.Count() is a marker method and should not be called directly.");

    /// <summary>Counts records for which the <paramref name="predicate"/> evaluates to <c>true</c>. KQL: <c>countif(predicate)</c>.</summary>
    /// <param name="predicate">A boolean expression to filter records.</param>
    public static long CountIf(bool predicate) => throw new InvalidOperationException("Kql.CountIf() is a marker method.");

    /// <summary>Calculates the sum of <paramref name="column"/>. KQL: <c>sum(column)</c>.</summary>
    /// <param name="column">The column to sum.</param>
    public static TValue Sum<TValue>(TValue column) => throw new InvalidOperationException("Kql.Sum() is a marker method.");

    /// <summary>Calculates the sum of <paramref name="column"/> for records where <paramref name="predicate"/> is <c>true</c>. KQL: <c>sumif(column, predicate)</c>.</summary>
    /// <param name="column">The column to sum.</param>
    /// <param name="predicate">A boolean filter expression.</param>
    public static TValue SumIf<TValue>(TValue column, bool predicate) => throw new InvalidOperationException("Kql.SumIf() is a marker method.");

    /// <summary>Calculates the average of <paramref name="column"/>. KQL: <c>avg(column)</c>.</summary>
    /// <param name="column">The numeric column.</param>
    public static double Avg<TValue>(TValue column) => throw new InvalidOperationException("Kql.Avg() is a marker method.");

    /// <summary>Returns the minimum value of <paramref name="column"/>. KQL: <c>min(column)</c>.</summary>
    /// <param name="column">The column to evaluate.</param>
    public static TValue Min<TValue>(TValue column) => throw new InvalidOperationException("Kql.Min() is a marker method.");

    /// <summary>Returns the maximum value of <paramref name="column"/>. KQL: <c>max(column)</c>.</summary>
    /// <param name="column">The column to evaluate.</param>
    public static TValue Max<TValue>(TValue column) => throw new InvalidOperationException("Kql.Max() is a marker method.");

    /// <summary>Calculates the approximate count of distinct values of <paramref name="column"/>. KQL: <c>dcount(column)</c>.</summary>
    /// <param name="column">The column whose distinct values are counted.</param>
    public static long DCount<TValue>(TValue column) => throw new InvalidOperationException("Kql.DCount() is a marker method.");

    /// <summary>Returns the value at the given <paramref name="percentile"/> for <paramref name="column"/>. KQL: <c>percentile(column, percentile)</c>.</summary>
    /// <param name="column">The column to evaluate.</param>
    /// <param name="percentile">The percentile value (0–100).</param>
    public static double Percentile<TValue>(TValue column, double percentile) => throw new InvalidOperationException("Kql.Percentile() is a marker method.");

    /// <summary>Returns values at multiple percentiles for <paramref name="column"/>. KQL: <c>percentiles(column, p1, p2, ...)</c>.</summary>
    /// <param name="column">The column to evaluate.</param>
    /// <param name="percentiles">One or more percentile values (0–100).</param>
    public static object Percentiles<TValue>(TValue column, params double[] percentiles) => throw new InvalidOperationException("Kql.Percentiles() is a marker method.");

    /// <summary>Collects all values of <paramref name="column"/> into a dynamic array. KQL: <c>make_list(column)</c>.</summary>
    /// <param name="column">The column whose values are collected.</param>
    public static IReadOnlyList<TValue> MakeList<TValue>(TValue column) => throw new InvalidOperationException("Kql.MakeList() is a marker method.");

    /// <summary>Collects values of <paramref name="column"/> into a dynamic array, limited to <paramref name="maxSize"/> elements. KQL: <c>make_list(column, maxSize)</c>.</summary>
    /// <param name="column">The column whose values are collected.</param>
    /// <param name="maxSize">Maximum number of elements to include.</param>
    public static IReadOnlyList<TValue> MakeList<TValue>(TValue column, int maxSize) => throw new InvalidOperationException("Kql.MakeList() is a marker method.");

    /// <summary>Collects distinct values of <paramref name="column"/> into a dynamic array. KQL: <c>make_set(column)</c>.</summary>
    /// <param name="column">The column whose distinct values are collected.</param>
    public static IReadOnlyList<TValue> MakeSet<TValue>(TValue column) => throw new InvalidOperationException("Kql.MakeSet() is a marker method.");

    /// <summary>Collects distinct values of <paramref name="column"/> into a dynamic array, limited to <paramref name="maxSize"/> elements. KQL: <c>make_set(column, maxSize)</c>.</summary>
    /// <param name="column">The column whose distinct values are collected.</param>
    /// <param name="maxSize">Maximum number of elements to include.</param>
    public static IReadOnlyList<TValue> MakeSet<TValue>(TValue column, int maxSize) => throw new InvalidOperationException("Kql.MakeSet() is a marker method.");

    /// <summary>Collects all key-value pairs into a dynamic property bag. KQL: <c>make_bag(column)</c>.</summary>
    /// <param name="column">The dynamic column to aggregate.</param>
    public static object MakeBag<TValue>(TValue column) => throw new InvalidOperationException("Kql.MakeBag() is a marker method.");

    /// <summary>Returns the row where <paramref name="column"/> is maximized, along with <paramref name="by"/>. KQL: <c>arg_max(column, by)</c>.</summary>
    /// <param name="column">The column to maximize.</param>
    /// <param name="by">Additional columns to return.</param>
    public static TValue ArgMax<TValue, TBy>(TValue column, TBy by) => throw new InvalidOperationException("Kql.ArgMax() is a marker method.");

    /// <summary>Returns the row where <paramref name="column"/> is minimized, along with <paramref name="by"/>. KQL: <c>arg_min(column, by)</c>.</summary>
    /// <param name="column">The column to minimize.</param>
    /// <param name="by">Additional columns to return.</param>
    public static TValue ArgMin<TValue, TBy>(TValue column, TBy by) => throw new InvalidOperationException("Kql.ArgMin() is a marker method.");

    /// <summary>Returns an arbitrary value from the group for <paramref name="column"/>. KQL: <c>take_any(column)</c>.</summary>
    /// <param name="column">The column to sample.</param>
    public static TValue TakeAny<TValue>(TValue column) => throw new InvalidOperationException("Kql.TakeAny() is a marker method.");

    /// <summary>Calculates the standard deviation of <paramref name="column"/>. KQL: <c>stdev(column)</c>.</summary>
    /// <param name="column">The numeric column.</param>
    public static double Stdev<TValue>(TValue column) => throw new InvalidOperationException("Kql.Stdev() is a marker method.");

    /// <summary>Calculates the variance of <paramref name="column"/>. KQL: <c>variance(column)</c>.</summary>
    /// <param name="column">The numeric column.</param>
    public static double Variance<TValue>(TValue column) => throw new InvalidOperationException("Kql.Variance() is a marker method.");

    // ─── Time functions ──────────────────────────────────────────────────

    /// <summary>Returns the datetime that is <paramref name="duration"/> before the current time. KQL: <c>ago(duration)</c>.</summary>
    /// <param name="duration">A <see cref="TimeSpan"/> representing how far back to go.</param>
    public static DateTime Ago(TimeSpan duration) => throw new InvalidOperationException("Kql.Ago() is a marker method.");

    /// <summary>Returns the current UTC date and time. KQL: <c>now()</c>.</summary>
    public static DateTime Now() => throw new InvalidOperationException("Kql.Now() is a marker method.");

    /// <summary>Rounds the <paramref name="column"/> value down to a multiple of <paramref name="roundTo"/>. KQL: <c>bin(column, roundTo)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    /// <param name="roundTo">The bin size as a <see cref="TimeSpan"/>.</param>
    public static DateTime Bin(DateTime column, TimeSpan roundTo) => throw new InvalidOperationException("Kql.Bin() is a marker method.");

    /// <summary>Returns the start of the day for <paramref name="column"/>. KQL: <c>startofday(column)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    public static DateTime StartOfDay(DateTime column) => throw new InvalidOperationException("Kql.StartOfDay() is a marker method.");

    /// <summary>Returns the start of the month for <paramref name="column"/>. KQL: <c>startofmonth(column)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    public static DateTime StartOfMonth(DateTime column) => throw new InvalidOperationException("Kql.StartOfMonth() is a marker method.");

    /// <summary>Returns the start of the week for <paramref name="column"/>. KQL: <c>startofweek(column)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    public static DateTime StartOfWeek(DateTime column) => throw new InvalidOperationException("Kql.StartOfWeek() is a marker method.");

    /// <summary>Returns the start of the year for <paramref name="column"/>. KQL: <c>startofyear(column)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    public static DateTime StartOfYear(DateTime column) => throw new InvalidOperationException("Kql.StartOfYear() is a marker method.");

    /// <summary>Returns the end of the day for <paramref name="column"/>. KQL: <c>endofday(column)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    public static DateTime EndOfDay(DateTime column) => throw new InvalidOperationException("Kql.EndOfDay() is a marker method.");

    /// <summary>Returns the end of the month for <paramref name="column"/>. KQL: <c>endofmonth(column)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    public static DateTime EndOfMonth(DateTime column) => throw new InvalidOperationException("Kql.EndOfMonth() is a marker method.");

    /// <summary>Returns the end of the week for <paramref name="column"/>. KQL: <c>endofweek(column)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    public static DateTime EndOfWeek(DateTime column) => throw new InvalidOperationException("Kql.EndOfWeek() is a marker method.");

    /// <summary>Returns the end of the year for <paramref name="column"/>. KQL: <c>endofyear(column)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    public static DateTime EndOfYear(DateTime column) => throw new InvalidOperationException("Kql.EndOfYear() is a marker method.");

    /// <summary>Computes the difference between two datetimes in the given <paramref name="period"/>. KQL: <c>datetime_diff(period, dt1, dt2)</c>.</summary>
    /// <param name="period">The unit of measurement (e.g. <c>"day"</c>, <c>"hour"</c>, <c>"minute"</c>, <c>"second"</c>).</param>
    /// <param name="dt1">The first datetime.</param>
    /// <param name="dt2">The second datetime.</param>
    public static long DatetimeDiff(string period, DateTime dt1, DateTime dt2) => throw new InvalidOperationException("Kql.DatetimeDiff() is a marker method.");

    /// <summary>Adds <paramref name="amount"/> units of <paramref name="period"/> to a datetime. KQL: <c>datetime_add(period, amount, datetime)</c>.</summary>
    /// <param name="period">The unit to add (e.g. <c>"day"</c>, <c>"hour"</c>, <c>"minute"</c>).</param>
    /// <param name="amount">The number of units to add (can be negative).</param>
    /// <param name="datetime">The datetime value.</param>
    public static DateTime DatetimeAdd(string period, long amount, DateTime datetime) => throw new InvalidOperationException("Kql.DatetimeAdd() is a marker method.");

    /// <summary>Returns the day of week as a <see cref="TimeSpan"/>. KQL: <c>dayofweek(column)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    public static TimeSpan DayOfWeek(DateTime column) => throw new InvalidOperationException("Kql.DayOfWeek() is a marker method.");

    /// <summary>Formats a datetime according to the given <paramref name="format"/> string. KQL: <c>format_datetime(column, format)</c>.</summary>
    /// <param name="column">The datetime column.</param>
    /// <param name="format">The format string (e.g. <c>"yyyy-MM-dd"</c>).</param>
    public static string FormatDatetime(DateTime column, string format) => throw new InvalidOperationException("Kql.FormatDatetime() is a marker method.");

    // ─── Scalar functions ────────────────────────────────────────────────

    /// <summary>Tests whether <paramref name="column"/> is between <paramref name="from"/> and <paramref name="to"/> (inclusive). KQL: <c>column between (from .. to)</c>.</summary>
    /// <param name="column">The column to test.</param>
    /// <param name="from">The lower bound (inclusive).</param>
    /// <param name="to">The upper bound (inclusive).</param>
    public static bool Between<T>(T column, T from, T to) => throw new InvalidOperationException("Kql.Between() is a marker method.");

    /// <summary>Returns <c>true</c> if the string <paramref name="column"/> is empty or null. KQL: <c>isempty(column)</c>.</summary>
    /// <param name="column">The string column.</param>
    public static bool IsEmpty(string column) => throw new InvalidOperationException("Kql.IsEmpty() is a marker method.");

    /// <summary>Returns <c>true</c> if the string <paramref name="column"/> is not empty and not null. KQL: <c>isnotempty(column)</c>.</summary>
    /// <param name="column">The string column.</param>
    public static bool IsNotEmpty(string column) => throw new InvalidOperationException("Kql.IsNotEmpty() is a marker method.");

    // ─── Type conversion functions ───────────────────────────────────────

    /// <summary>Converts <paramref name="value"/> to a <c>long</c>. KQL: <c>tolong(value)</c>.</summary>
    /// <param name="value">The value to convert.</param>
    public static long ToLong<T>(T value) => throw new InvalidOperationException("Kql.ToLong() is a marker method.");

    /// <summary>Converts <paramref name="value"/> to an <c>int</c>. KQL: <c>toint(value)</c>.</summary>
    /// <param name="value">The value to convert.</param>
    public static int ToInt<T>(T value) => throw new InvalidOperationException("Kql.ToInt() is a marker method.");

    /// <summary>Converts <paramref name="value"/> to a <c>double</c>. KQL: <c>todouble(value)</c>.</summary>
    /// <param name="value">The value to convert.</param>
    public static double ToDouble<T>(T value) => throw new InvalidOperationException("Kql.ToDouble() is a marker method.");

    /// <summary>Converts <paramref name="value"/> to a <c>real</c>. KQL: <c>toreal(value)</c>.</summary>
    /// <param name="value">The value to convert.</param>
    public static double ToReal<T>(T value) => throw new InvalidOperationException("Kql.ToReal() is a marker method.");

    /// <summary>Converts <paramref name="value"/> to a <c>string</c>. KQL: <c>tostring(value)</c>.</summary>
    /// <param name="value">The value to convert.</param>
    public static string ToString<T>(T value) => throw new InvalidOperationException("Kql.ToString() is a marker method.");

    /// <summary>Converts <paramref name="value"/> to a <c>datetime</c>. KQL: <c>todatetime(value)</c>.</summary>
    /// <param name="value">The value to convert.</param>
    public static DateTime ToDateTime<T>(T value) => throw new InvalidOperationException("Kql.ToDateTime() is a marker method.");

    /// <summary>Converts <paramref name="value"/> to a <c>timespan</c>. KQL: <c>totimespan(value)</c>.</summary>
    /// <param name="value">The value to convert.</param>
    public static TimeSpan ToTimeSpan<T>(T value) => throw new InvalidOperationException("Kql.ToTimeSpan() is a marker method.");

    // ─── String functions ────────────────────────────────────────────────

    /// <summary>Returns the length of the string. KQL: <c>strlen(value)</c>.</summary>
    /// <param name="value">The string value.</param>
    public static int Strlen(string value) => throw new InvalidOperationException("Kql.Strlen() is a marker method.");

    /// <summary>Extracts a substring. KQL: <c>substring(value, start, length)</c>.</summary>
    /// <param name="value">The source string.</param>
    /// <param name="start">The zero-based start index.</param>
    /// <param name="length">The number of characters to extract.</param>
    public static string Substring(string value, int start, int length) => throw new InvalidOperationException("Kql.Substring() is a marker method.");

    /// <summary>Removes leading and trailing matches of <paramref name="regex"/>. KQL: <c>trim(regex, value)</c>.</summary>
    /// <param name="regex">A regular expression pattern to trim.</param>
    /// <param name="value">The string to trim.</param>
    public static string Trim(string regex, string value) => throw new InvalidOperationException("Kql.Trim() is a marker method.");

    /// <summary>Converts a string to upper case. KQL: <c>toupper(value)</c>.</summary>
    /// <param name="value">The string value.</param>
    public static string ToUpper(string value) => throw new InvalidOperationException("Kql.ToUpper() is a marker method.");

    /// <summary>Converts a string to lower case. KQL: <c>tolower(value)</c>.</summary>
    /// <param name="value">The string value.</param>
    public static string ToLower(string value) => throw new InvalidOperationException("Kql.ToLower() is a marker method.");

    /// <summary>Concatenates strings. KQL: <c>strcat(values...)</c>.</summary>
    /// <param name="values">The strings to concatenate.</param>
    public static string Strcat(params string[] values) => throw new InvalidOperationException("Kql.Strcat() is a marker method.");

    /// <summary>Extracts a match for a regular expression from a string. KQL: <c>extract(regex, captureGroup, source)</c>.</summary>
    /// <param name="regex">The regular expression pattern.</param>
    /// <param name="captureGroup">The capture group index (0 = entire match).</param>
    /// <param name="source">The string to search.</param>
    public static string Extract(string regex, int captureGroup, string source) => throw new InvalidOperationException("Kql.Extract() is a marker method.");

    /// <summary>Splits a string by a delimiter into a dynamic array. KQL: <c>split(source, delimiter)</c>.</summary>
    /// <param name="source">The string to split.</param>
    /// <param name="delimiter">The delimiter string.</param>
    public static object Split(string source, string delimiter) => throw new InvalidOperationException("Kql.Split() is a marker method.");

    /// <summary>Replaces all occurrences of a string with another. KQL: <c>replace_string(source, lookup, rewrite)</c>.</summary>
    /// <param name="source">The original string.</param>
    /// <param name="lookup">The substring to find.</param>
    /// <param name="rewrite">The replacement string.</param>
    public static string ReplaceString(string source, string lookup, string rewrite) => throw new InvalidOperationException("Kql.ReplaceString() is a marker method.");

    /// <summary>Replaces all regex matches with a rewrite pattern. KQL: <c>replace_regex(source, regex, rewrite)</c>.</summary>
    /// <param name="source">The original string.</param>
    /// <param name="regex">The regular expression pattern.</param>
    /// <param name="rewrite">The replacement pattern.</param>
    public static string ReplaceRegex(string source, string regex, string rewrite) => throw new InvalidOperationException("Kql.ReplaceRegex() is a marker method.");

    /// <summary>Returns the zero-based position of a substring. KQL: <c>indexof(source, lookup)</c>.</summary>
    /// <param name="source">The string to search in.</param>
    /// <param name="lookup">The substring to find.</param>
    public static long IndexOf(string source, string lookup) => throw new InvalidOperationException("Kql.IndexOf() is a marker method.");

    // ─── Dynamic / JSON functions ────────────────────────────────────────

    /// <summary>Parses a string as JSON and returns a dynamic value. KQL: <c>parse_json(value)</c>.</summary>
    /// <param name="value">The value to parse as JSON.</param>
    public static object ParseJson<T>(T value) => throw new InvalidOperationException("Kql.ParseJson() is a marker method.");

    /// <summary>Returns the number of elements in a dynamic array. KQL: <c>array_length(column)</c>.</summary>
    /// <param name="column">The dynamic array column.</param>
    public static long ArrayLength<T>(T column) => throw new InvalidOperationException("Kql.ArrayLength() is a marker method.");

    /// <summary>Creates a dynamic property bag from alternating key-value pairs. KQL: <c>pack(key1, value1, ...)</c>.</summary>
    /// <param name="values">Alternating keys and values.</param>
    public static object Pack(params object[] values) => throw new InvalidOperationException("Kql.Pack() is a marker method.");

    /// <summary>Returns the keys of a dynamic property bag. KQL: <c>bag_keys(column)</c>.</summary>
    /// <param name="column">The dynamic property bag column.</param>
    public static object BagKeys<T>(T column) => throw new InvalidOperationException("Kql.BagKeys() is a marker method.");

    // ─── Conditional functions ───────────────────────────────────────────

    /// <summary>Returns <paramref name="ifTrue"/> if the <paramref name="predicate"/> is <c>true</c>, otherwise <paramref name="ifFalse"/>. KQL: <c>iff(predicate, ifTrue, ifFalse)</c>.</summary>
    /// <param name="predicate">The boolean expression.</param>
    /// <param name="ifTrue">Value returned when true.</param>
    /// <param name="ifFalse">Value returned when false.</param>
    public static TValue Iff<TValue>(bool predicate, TValue ifTrue, TValue ifFalse) => throw new InvalidOperationException("Kql.Iff() is a marker method.");

    /// <summary>Returns the first non-null value from the arguments. KQL: <c>coalesce(values...)</c>.</summary>
    /// <param name="values">The values to evaluate.</param>
    public static TValue Coalesce<TValue>(params TValue[] values) => throw new InvalidOperationException("Kql.Coalesce() is a marker method.");
}
