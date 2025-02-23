using EtlApp.Domain.Config;

namespace EtlApp.Adapter.Sql;

public class SqlTargetConfig: TargetConfig
{
    public required string ConnectionName { get; set; }
    public required string TableName { get; set; }
    public string? ArchiveTableName { get; set; }
    public string? StagingTableName { get; set; }
}