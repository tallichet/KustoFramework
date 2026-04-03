using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using KustoFramework.Attributes;

namespace KustoFramework.Azure.Materialization;

/// <summary>
/// Maps <see cref="IDataReader"/> rows to instances of <typeparamref name="T"/>
/// using property names and <see cref="KqlColumnAttribute"/> mappings.
/// </summary>
internal static class KqlResultMapper<T> where T : new()
{
    private static readonly ConcurrentDictionary<Type, PropertyMapping[]> MappingCache = new();

    /// <summary>
    /// Materializes all rows from the reader into a list of <typeparamref name="T"/>.
    /// </summary>
    public static List<T> MapAll(IDataReader reader)
    {
        var mappings = BuildMappings(reader);
        var results = new List<T>();

        while (reader.Read())
        {
            var item = new T();
            for (var i = 0; i < mappings.Length; i++)
            {
                var mapping = mappings[i];
                if (mapping.Ordinal < 0)
                    continue;

                var value = reader.GetValue(mapping.Ordinal);
                if (value is DBNull)
                    continue;

                var converted = ConvertValue(value, mapping.Property.PropertyType);
                mapping.Property.SetValue(item, converted);
            }

            results.Add(item);
        }

        return results;
    }

    private static PropertyMapping[] BuildMappings(IDataReader reader)
    {
        var properties = GetCachedProperties(typeof(T));
        var columnLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < reader.FieldCount; i++)
        {
            columnLookup[reader.GetName(i)] = i;
        }

        var mappings = new PropertyMapping[properties.Length];
        for (var i = 0; i < properties.Length; i++)
        {
            var (columnName, prop) = properties[i];
            columnLookup.TryGetValue(columnName, out var ordinal);
            mappings[i] = new PropertyMapping(prop, columnLookup.ContainsKey(columnName) ? ordinal : -1);
        }

        return mappings;
    }

    private static (string ColumnName, PropertyInfo Property)[] GetCachedProperties(Type type)
    {
        // We cache the property → column name resolution, not the ordinal mapping (which depends on the query)
        return MappingCache.GetOrAdd(type, static t =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .Select(p =>
                {
                    var attr = p.GetCustomAttribute<KqlColumnAttribute>();
                    var columnName = attr?.Name ?? p.Name;
                    return new PropertyMapping(p, -1) { ColumnName = columnName };
                })
                .ToArray()
        ).Select(m => (m.ColumnName!, m.Property)).ToArray();
    }

    private static object? ConvertValue(object value, Type targetType)
    {
        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlying.IsAssignableFrom(value.GetType()))
            return value;

        if (underlying == typeof(TimeSpan) && value is string timeStr)
            return TimeSpan.Parse(timeStr);

        if (underlying == typeof(Guid) && value is string guidStr)
            return Guid.Parse(guidStr);

        return Convert.ChangeType(value, underlying);
    }

    private sealed class PropertyMapping(PropertyInfo property, int ordinal)
    {
        public PropertyInfo Property { get; } = property;
        public int Ordinal { get; } = ordinal;
        public string? ColumnName { get; init; }
    }
}
