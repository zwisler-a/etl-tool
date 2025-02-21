using EtlApp.Domain.Config;

namespace EtlApp.Adapter.Sql;

public class SqlSourceConfig : SourceConfig
{
    public required string ConnectionName { get; set; }
    public required string TableName { get; set; }
}