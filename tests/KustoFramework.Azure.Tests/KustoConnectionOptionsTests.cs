namespace KustoFramework.Azure.Tests;

public class KustoConnectionOptionsTests
{
    [Fact]
    public void BuildConnectionString_WithValidOptions_Succeeds()
    {
        var options = new KustoConnectionOptions
        {
            ClusterUri = "https://mycluster.kusto.windows.net",
            Database = "MyDatabase"
        };

        var kcsb = options.BuildConnectionString();

        Assert.NotNull(kcsb);
    }

    [Fact]
    public void BuildConnectionString_MissingClusterUri_Throws()
    {
        var options = new KustoConnectionOptions
        {
            Database = "MyDatabase"
        };

        var ex = Assert.Throws<InvalidOperationException>(() => options.BuildConnectionString());
        Assert.Contains("ClusterUri", ex.Message);
    }

    [Fact]
    public void BuildConnectionString_MissingDatabase_Throws()
    {
        var options = new KustoConnectionOptions
        {
            ClusterUri = "https://mycluster.kusto.windows.net"
        };

        var ex = Assert.Throws<InvalidOperationException>(() => options.BuildConnectionString());
        Assert.Contains("Database", ex.Message);
    }

    [Fact]
    public void BuildConnectionString_ConfigureConnectionCallbackInvoked()
    {
        var callbackInvoked = false;
        var options = new KustoConnectionOptions
        {
            ClusterUri = "https://mycluster.kusto.windows.net",
            Database = "MyDatabase",
            ConfigureConnection = _ => callbackInvoked = true
        };

        options.BuildConnectionString();

        Assert.True(callbackInvoked);
    }
}
