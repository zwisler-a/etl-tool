using EtlApp.Domain.Config;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Module;

namespace EtlApp.Domain.Execution;

public class PipelineBuilder(ModuleRegistry moduleRegistry)
{
    public Pipeline Build(List<SourceConfig> sourceConfig, List<TargetConfig> targetConfig,
        List<TransformerConfig> transformerConfigs, MappingConfig mappingConfig,
        List<MiddlewareConfig> middlewareConfig)
    {
        var globalTransformer = ConfigurationManager.Config.Transformer ?? [];
        var allTransformerConfigs = transformerConfigs.Concat(globalTransformer);
        var contextTransformers = new Dictionary<string, ITransformer>();
        var context = new PipelineContext(moduleRegistry.DatabaseManager, contextTransformers, mappingConfig);
        var transformers = allTransformerConfigs.ToDictionary(
            s => s.Name,
            (s) => moduleRegistry.TransformerFactory.Create(s, context)
        );
        transformers.ToList().ForEach(kvp => contextTransformers[kvp.Key] = kvp.Value);

        var middlewares = middlewareConfig
            .Select(m => moduleRegistry.MiddlewareFactory.Create(m, context)).ToList();
        var sources = sourceConfig
            .Select(s => moduleRegistry.SourceFactory.Create(s, context)).ToList();
        var targets = targetConfig
            .Select(s => moduleRegistry.TargetFactory.Create(s, context)).ToList();

        return new Pipeline(middlewares, sources, targets);
    }
}