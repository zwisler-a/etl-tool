using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Connection;

namespace EtlApp.Domain.Dto;

public record PipelineContext(
    DatabaseManager DatabaseManager,
    Dictionary<string, ITransformer> Transformers,
    List<ColumnMappingConfig> MappingConfig
)
{
    public ColumnMappingConfig GetColumnMapping(string columnName, ColumnType fallbackType = ColumnType.Undefined)
    {
        var source =
            MappingConfig.Find(mappingConfig => mappingConfig.SourceName.Equals(columnName));
        return source ?? new ColumnMappingConfig
            { SourceName = columnName, SourceType = fallbackType, TargetName = columnName };
    }
};