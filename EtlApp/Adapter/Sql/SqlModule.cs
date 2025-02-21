using EtlApp.Domain.Database;
using EtlApp.Domain.Module;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;
using Microsoft.Data.SqlClient;

namespace EtlApp.Adapter.Sql;

public class SqlModule : Module
{
    public void RegisterSourceConnection(SourceConnectionFactory factory)
    {
        // No Source
    }

    public void RegisterTargetConnection(TargetConnectionFactory factory)
    {
        factory.Register((SqlTargetConfig config) => new SqlTargetConnection(config));
    }

    public void RegisterConnections(DatabaseManager databaseManager)
    {
        var connection = new SqlConnection();
        connection.ConnectionString = "Server=localhost;Database=TestDb;User Id=sa;Password=ilujhasdhas73679zhansdjASDalsdjhasjdn;Encrypt=False;";
        databaseManager.RegisterConnection("sql", connection);
    }
}