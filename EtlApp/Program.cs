using CommandLine;
using EtlApp.Adapter.BuildIn;
using EtlApp.Adapter.Csv;
using EtlApp.Adapter.Sql;
using EtlApp.Domain.Config;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Execution;
using EtlApp.Domain.Module;
using Microsoft.Extensions.Logging;

namespace EtlApp;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                ConfigurationManager.ConfigFilePath = options.ApplicationConfigPath;
                if (options.Verbose)
                    Logging.SetLoggingLevel(LogLevel.Debug);
                ILogger<Program> logger = Logging.LoggerFactory.CreateLogger<Program>();

                var moduleRegistry = new ModuleRegistry();
                var executionService = new PipelineBuilder(moduleRegistry);
                moduleRegistry.RegisterModule(new BuildInModule());
                moduleRegistry.RegisterModule(new CsvModule());
                moduleRegistry.RegisterModule(new SqlModule());

                var files = Directory.GetFiles(ConfigurationManager.Config.ConfigFolder,
                    options.IncludePipeline ?? "*.json");
                logger.LogInformation($"Found {files.Length} pipelines");
                var pipelines = files.Select(file =>
                {
                    var config = ConfigurationManager.LoadPipelineConfiguration(file);
                    var pipeline = executionService.Build(
                        config.Source,
                        config.Target,
                        config.Transformer,
                        config.Mapping ?? new MappingConfig { Mappings = [] },
                        config.Middleware
                    );
                    return pipeline;
                });
                Parallel.ForEach(pipelines, pipeline =>
                {
                    logger.LogDebug($"Processing pipeline: {pipeline}");
                    pipeline.Run();
                });
            });
    }
}