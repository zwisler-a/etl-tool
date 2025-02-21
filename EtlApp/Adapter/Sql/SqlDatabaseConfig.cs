using EtlApp.Domain.Config;

namespace EtlApp.Adapter.Sql;

public class SqlDatabaseConfig: DatabaseConfig
{
    public required string ConnectionString { get; set; }
}