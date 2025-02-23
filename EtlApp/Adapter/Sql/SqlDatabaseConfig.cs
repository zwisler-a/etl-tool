using EtlApp.Domain.Config.Pipeline;

namespace EtlApp.Adapter.Sql;

public class SqlDatabaseConfig: DatabaseConfig
{
    public required string ConnectionString { get; set; }
}