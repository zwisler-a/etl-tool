using System.Collections;
using EtlApp.Domain.Config;
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
        SourceConfigConverter.Register<SqlSourceConfig>("sql");
        factory.Register((SqlSourceConfig config) => new SqlSourceConnection(config));
    }

    public void RegisterTargetConnection(TargetConnectionFactory factory)
    {
        TargetConfigConverter.Register<SqlTargetConfig>("sql");
        factory.Register((SqlTargetConfig config) => new SqlTargetConnection(config));
    }

    public void RegisterConnections(DatabaseManager databaseManager)
    {
        DatabaseConfigConverter.Register<SqlDatabaseConfig>("sql");

        var configs = ConfigurationManager.Config.Database
            .Where(s => s is SqlDatabaseConfig)
            .Cast<SqlDatabaseConfig>();

        foreach (var databaseConfig in configs)
        {
            var connection = new SqlConnection();
            connection.ConnectionString = databaseConfig.ConnectionString;
            connection.Open();
            databaseManager.RegisterConnection(databaseConfig.Type, connection);
        }
    }
}