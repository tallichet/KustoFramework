using System.Data;
using KustoFramework.Azure.Materialization;

namespace KustoFramework.Azure.Tests;

public class ResultMapperTests
{
    [Fact]
    public void MapAll_BasicTypes_MapsCorrectly()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Value", typeof(double));
        table.Columns.Add("Timestamp", typeof(DateTime));
        table.Columns.Add("IsActive", typeof(bool));
        table.Columns.Add("Count", typeof(long));

        var now = DateTime.UtcNow;
        table.Rows.Add(1, "Alice", 9.5, now, true, 100L);
        table.Rows.Add(2, "Bob", 4.2, now.AddHours(-1), false, 200L);

        using var reader = table.CreateDataReader();
        var results = KqlResultMapper<SimpleRecord>.MapAll(reader);

        Assert.Equal(2, results.Count);

        Assert.Equal(1, results[0].Id);
        Assert.Equal("Alice", results[0].Name);
        Assert.Equal(9.5, results[0].Value);
        Assert.Equal(now, results[0].Timestamp);
        Assert.True(results[0].IsActive);
        Assert.Equal(100L, results[0].Count);

        Assert.Equal(2, results[1].Id);
        Assert.Equal("Bob", results[1].Name);
    }

    [Fact]
    public void MapAll_WithKqlColumnAttribute_UsesAttributeName()
    {
        var table = new DataTable();
        table.Columns.Add("record_id", typeof(int));
        table.Columns.Add("display_name", typeof(string));

        table.Rows.Add(42, "Test Record");

        using var reader = table.CreateDataReader();
        var results = KqlResultMapper<MappedRecord>.MapAll(reader);

        Assert.Single(results);
        Assert.Equal(42, results[0].Id);
        Assert.Equal("Test Record", results[0].Name);
    }

    [Fact]
    public void MapAll_WithDBNull_LeavesDefaultValue()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Value", typeof(double));
        table.Columns.Add("Timestamp", typeof(DateTime));

        table.Rows.Add(1, DBNull.Value, DBNull.Value, DBNull.Value);

        using var reader = table.CreateDataReader();
        var results = KqlResultMapper<NullableRecord>.MapAll(reader);

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
        Assert.Null(results[0].Name);
        Assert.Null(results[0].Value);
        Assert.Null(results[0].Timestamp);
    }

    [Fact]
    public void MapAll_EmptyReader_ReturnsEmptyList()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));

        using var reader = table.CreateDataReader();
        var results = KqlResultMapper<SimpleRecord>.MapAll(reader);

        Assert.Empty(results);
    }

    [Fact]
    public void MapAll_ExtraColumnsInReader_AreIgnored()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("UnknownColumn", typeof(string));

        table.Rows.Add(1, "Test", "extra");

        using var reader = table.CreateDataReader();
        var results = KqlResultMapper<SimpleRecord>.MapAll(reader);

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
        Assert.Equal("Test", results[0].Name);
    }

    [Fact]
    public void MapAll_MissingColumnsInReader_LeavesDefaults()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        // Name and Value columns are missing

        table.Rows.Add(1);

        using var reader = table.CreateDataReader();
        var results = KqlResultMapper<SimpleRecord>.MapAll(reader);

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
        Assert.Equal("", results[0].Name);
        Assert.Equal(0.0, results[0].Value);
    }

    [Fact]
    public void MapAll_CaseInsensitiveColumnMatching()
    {
        var table = new DataTable();
        table.Columns.Add("id", typeof(int));
        table.Columns.Add("NAME", typeof(string));

        table.Rows.Add(1, "CaseTest");

        using var reader = table.CreateDataReader();
        var results = KqlResultMapper<SimpleRecord>.MapAll(reader);

        Assert.Single(results);
        Assert.Equal(1, results[0].Id);
        Assert.Equal("CaseTest", results[0].Name);
    }

    [Fact]
    public void MapAll_GuidColumn_MapsCorrectly()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("TraceId", typeof(Guid));

        var guid = Guid.NewGuid();
        table.Rows.Add(1, "Test", guid);

        using var reader = table.CreateDataReader();
        var results = KqlResultMapper<SimpleRecord>.MapAll(reader);

        Assert.Single(results);
        Assert.Equal(guid, results[0].TraceId);
    }

    [Fact]
    public void MapAll_GuidAsString_ParsesCorrectly()
    {
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("TraceId", typeof(string));

        var guid = Guid.NewGuid();
        table.Rows.Add(1, "Test", guid.ToString());

        using var reader = table.CreateDataReader();
        var results = KqlResultMapper<SimpleRecord>.MapAll(reader);

        Assert.Single(results);
        Assert.Equal(guid, results[0].TraceId);
    }
}
