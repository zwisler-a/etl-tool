using EtlApp.Domain.Dto;

namespace EtlApp.Domain.Config;

public class ColumnMappingConfig
{
    public required string SourceName { get; set; }
    private string? _targetName;

    public string? TargetName
    {
        get => _targetName ?? SourceName;
        set => _targetName = value;
    }

    public ColumnType SourceType { get; set; } = ColumnType.Undefined;
    public List<string>? Transform { get; set; }
    public List<ColumnModifiers>? Modifiers { get; set; }
}