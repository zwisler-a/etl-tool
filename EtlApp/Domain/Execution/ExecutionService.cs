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

    public Pipeline Build(SourceConfig sourceConfig, TargetConfig targetConfig, MappingConfig mappingConfig,
        List<IMiddleware> middlewares)
    {
        var source = moduleRegistry.SourceFactory.Create(sourceConfig);
        var target = moduleRegistry.TargetFactory.Create(targetConfig);
        var context = new PipelineExecutionContext(moduleRegistry.DatabaseManager, null, mappingConfig);
        return new Pipeline(source, target, context, middlewares);
    }
}