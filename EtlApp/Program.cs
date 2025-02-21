using EtlApp.Adapter.Csv;
using EtlApp.Adapter.Sql;
using EtlApp.Domain.Config;
using EtlApp.Domain.Execution;
using EtlApp.Domain.Middleware;
using EtlApp.Domain.Module;
using Microsoft.Extensions.Logging;


class Program
{
    
    private static ILogger<Program> _logger = Logging.LoggerFactory.CreateLogger<Program>();
    
    static void Main(string[] args)
    {
        var middlewareManager = new MiddlewareManager();
        middlewareManager.Register("type_inference", new DetectTypeMiddleware(true));
        middlewareManager.Register("type_cast", new CastTypeMiddleware());
        middlewareManager.Register("type_validation", new ValidateTypeMiddleware());

        ConfigurationManager.ConfigFilePath =
            "C:\\Users\\Alex\\RiderProjects\\EtlApp\\EtlApp\\TestConfig\\application_config.json";

        var moduleRegistry = new ModuleRegistry();
        var executionService = new ExecutionService(moduleRegistry);
        moduleRegistry.RegisterModule(new CsvModule());
        moduleRegistry.RegisterModule(new SqlModule());


        var files = Directory.GetFiles(ConfigurationManager.Config.ConfigFolder, "*_pipeline.json");
        _logger.LogInformation($"Found {files.Length} pipelines");
        var pipelines = files.Select(file =>
        {
            var config = ConfigurationManager.LoadPipelineConfiguration(file);
            var pipeline = executionService.Build(
                config.Source,
                config.Target,
                new MappingConfig { Mappings = [] },
                config.Middleware.Select(m => middlewareManager.Get(m)).ToList()!
            );
            return pipeline;
        });

        foreach (var pipeline in pipelines)
        {
            _logger.LogDebug($"Processing pipeline: {pipeline}");
            pipeline.Run();
        }
    }
}