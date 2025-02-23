using EtlApp.Adapter.BuildIn.Middlewares;

namespace EtlApp.Domain.Config.Pipeline;

public class PipelineConfiguration
{
    public required List<SourceConfig> Source { get; set; }
    public required List<TargetConfig> Target { get; set; }
    public List<TransformerConfig> Transformer { get; set; } = [];

    public List<MiddlewareConfig> Middleware { get; set; } =
    [
        new ApplyTransformMiddlewareConfig { Type = "apply_transformers" },
        new DetectTypeMiddlewareConfig { Type = "type_inference" },
        new CastTypeMiddlewareConfig { Type = "type_cast" },
        new ValidateTypeMiddlewareConfig { Type = "type_validation" }
    ];

    public MappingConfig? Mapping { get; set; }
}