using EtlApp.Domain.Config;
using EtlApp.Domain.Config.Pipeline.Converter;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Module;
using Microsoft.Data.SqlClient;

namespace EtlApp.Adapter.Sql;

public class SqlModule : Module
{
    public override void RegisterSourceConnection(SourceConnectionFactory factory)
    {
        SourceConfigConverter.Register<SqlSourceConfig>("sql");
        factory.Register((SqlSourceConfig config, PipelineContext context) => new SqlSourceConnection(config, context));
    }

    public override void RegisterTargetConnection(TargetConnectionFactory factory)
    {
        TargetConfigConverter.Register<SqlTargetConfig>("sql");
        factory.Register((SqlTargetConfig config, PipelineContext context) =>
            new SqlTargetConnection(config, context));
    }

    public override void RegisterConnections(DatabaseManager databaseManager)
    {
        DatabaseConfigConverter.Register<SqlDatabaseConfig>("sql");

        var configs = ConfigurationManager.Config.Database
            .Where(s => s is SqlDatabaseConfig)
            .Cast<SqlDatabaseConfig>();

        foreach (var databaseConfig in configs)
        {
            var connection = new SqlConnection();
            connection.ConnectionString = databaseConfig.ConnectionString;
            databaseManager.RegisterConnection(databaseConfig.Type, connection);
        }
    }
}