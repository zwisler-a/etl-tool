using EtlApp.Domain.Config;

namespace EtlApp.Adapter.Sql;

public class SqlTargetConfig: TargetConfig
{
    public required string ConnectionName { get; set; }
    public required string TableName { get; set; }
}