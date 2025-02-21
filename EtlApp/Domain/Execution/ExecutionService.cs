using EtlApp.Domain.Config;
using EtlApp.Domain.Middleware;
using EtlApp.Domain.Module;

namespace EtlApp.Domain.Execution;

public class ExecutionService(ModuleRegistry moduleRegistry)
{
    public void Execute(Pipeline pipeline)
    {
        pipeline.Run();
    }

    public Pipeline Build(List<SourceConfig> sourceConfig, List<TargetConfig> targetConfig, MappingConfig mappingConfig,
        List<IMiddleware> middlewares)
    {
        var sources = sourceConfig.Select(s => moduleRegistry.SourceFactory.Create(s)).ToList();
        var targets = targetConfig.Select(s => moduleRegistry.TargetFactory.Create(s)).ToList();
        var context = new PipelineExecutionContext(moduleRegistry.DatabaseManager, null, mappingConfig);
        return new Pipeline(context, middlewares, sources, targets);
    }
}