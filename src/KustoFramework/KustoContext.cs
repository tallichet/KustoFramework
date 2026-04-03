using KustoFramework.Attributes;
using KustoFramework.Query;
using System.Reflection;

namespace KustoFramework;

public class KustoContext
{
    public KqlQuery<T> Table<T>()
    {
        var tableName = KqlQuery<T>.ResolveTableName();
        return new KqlQuery<T>(tableName);
    }

    public KqlQuery<T> Table<T>(string tableName) =>
        new KqlQuery<T>(tableName);
}
