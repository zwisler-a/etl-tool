using EtlApp.Domain.Dto;

namespace EtlApp.Domain.Config;

public class ColumnMappingConfig
{
    public required string SourceName { get; set; }
    public required string TargetName { get; set; }
    public required ColumnType SourceType { get; set; }
}