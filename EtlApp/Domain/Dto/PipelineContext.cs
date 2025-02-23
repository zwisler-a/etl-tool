using EtlApp.Domain.Config;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Database;

namespace EtlApp.Domain.Dto;

public record PipelineContext(
    DatabaseManager DatabaseManager,
    Dictionary<string, ITransformer> Transformers,
    MappingConfig MappingConfig
);