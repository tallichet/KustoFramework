using System.Linq.Expressions;

namespace KustoFramework.Functions;

/// <summary>
/// Static marker methods for KQL functions. These methods are never executed at runtime —
/// they are intercepted by the expression visitor and translated to KQL syntax.
/// </summary>
public static class Kql
{
    // Aggregation functions
    public static long Count() => throw new InvalidOperationException("Kql.Count() is a marker method and should not be called directly.");
    public static long CountIf(bool predicate) => throw new InvalidOperationException("Kql.CountIf() is a marker method.");
    public static TValue Sum<TValue>(TValue column) => throw new InvalidOperationException("Kql.Sum() is a marker method.");
    public static TValue SumIf<TValue>(TValue column, bool predicate) => throw new InvalidOperationException("Kql.SumIf() is a marker method.");
    public static double Avg<TValue>(TValue column) => throw new InvalidOperationException("Kql.Avg() is a marker method.");
    public static TValue Min<TValue>(TValue column) => throw new InvalidOperationException("Kql.Min() is a marker method.");
    public static TValue Max<TValue>(TValue column) => throw new InvalidOperationException("Kql.Max() is a marker method.");
    public static long DCount<TValue>(TValue column) => throw new InvalidOperationException("Kql.DCount() is a marker method.");
    public static double Percentile<TValue>(TValue column, double percentile) => throw new InvalidOperationException("Kql.Percentile() is a marker method.");
    public static IReadOnlyList<TValue> MakeList<TValue>(TValue column) => throw new InvalidOperationException("Kql.MakeList() is a marker method.");
    public static IReadOnlyList<TValue> MakeSet<TValue>(TValue column) => throw new InvalidOperationException("Kql.MakeSet() is a marker method.");
    public static TValue ArgMax<TValue, TBy>(TValue column, TBy by) => throw new InvalidOperationException("Kql.ArgMax() is a marker method.");
    public static TValue ArgMin<TValue, TBy>(TValue column, TBy by) => throw new InvalidOperationException("Kql.ArgMin() is a marker method.");

    // Time functions
    public static DateTime Ago(TimeSpan duration) => throw new InvalidOperationException("Kql.Ago() is a marker method.");
    public static DateTime Now() => throw new InvalidOperationException("Kql.Now() is a marker method.");
    public static DateTime Bin(DateTime column, TimeSpan roundTo) => throw new InvalidOperationException("Kql.Bin() is a marker method.");
    public static DateTime StartOfDay(DateTime column) => throw new InvalidOperationException("Kql.StartOfDay() is a marker method.");
    public static DateTime StartOfMonth(DateTime column) => throw new InvalidOperationException("Kql.StartOfMonth() is a marker method.");
    public static DateTime StartOfWeek(DateTime column) => throw new InvalidOperationException("Kql.StartOfWeek() is a marker method.");
    public static DateTime StartOfYear(DateTime column) => throw new InvalidOperationException("Kql.StartOfYear() is a marker method.");

    // Scalar functions
    public static bool Between<T>(T column, T from, T to) => throw new InvalidOperationException("Kql.Between() is a marker method.");
    public static bool IsEmpty(string column) => throw new InvalidOperationException("Kql.IsEmpty() is a marker method.");
    public static bool IsNotEmpty(string column) => throw new InvalidOperationException("Kql.IsNotEmpty() is a marker method.");

    // Type conversion functions
    public static long ToLong<T>(T value) => throw new InvalidOperationException("Kql.ToLong() is a marker method.");
    public static int ToInt<T>(T value) => throw new InvalidOperationException("Kql.ToInt() is a marker method.");
    public static double ToDouble<T>(T value) => throw new InvalidOperationException("Kql.ToDouble() is a marker method.");
    public static double ToReal<T>(T value) => throw new InvalidOperationException("Kql.ToReal() is a marker method.");
    public static string ToString<T>(T value) => throw new InvalidOperationException("Kql.ToString() is a marker method.");
    public static DateTime ToDateTime<T>(T value) => throw new InvalidOperationException("Kql.ToDateTime() is a marker method.");
    public static TimeSpan ToTimeSpan<T>(T value) => throw new InvalidOperationException("Kql.ToTimeSpan() is a marker method.");

    // String functions
    public static int Strlen(string value) => throw new InvalidOperationException("Kql.Strlen() is a marker method.");
    public static string Substring(string value, int start, int length) => throw new InvalidOperationException("Kql.Substring() is a marker method.");
    public static string Trim(string regex, string value) => throw new InvalidOperationException("Kql.Trim() is a marker method.");
    public static string ToUpper(string value) => throw new InvalidOperationException("Kql.ToUpper() is a marker method.");
    public static string ToLower(string value) => throw new InvalidOperationException("Kql.ToLower() is a marker method.");
    public static string Strcat(params string[] values) => throw new InvalidOperationException("Kql.Strcat() is a marker method.");

    // Conditional
    public static TValue Iff<TValue>(bool predicate, TValue ifTrue, TValue ifFalse) => throw new InvalidOperationException("Kql.Iff() is a marker method.");
    public static TValue Coalesce<TValue>(params TValue[] values) => throw new InvalidOperationException("Kql.Coalesce() is a marker method.");
}
