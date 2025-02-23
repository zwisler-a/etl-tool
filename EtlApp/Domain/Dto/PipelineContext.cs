using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Connection;

namespace EtlApp.Domain.Dto;

public record PipelineContext(
    DatabaseManager DatabaseManager,
    Dictionary<string, ITransformer> Transformers,
    MappingConfig MappingConfig
);