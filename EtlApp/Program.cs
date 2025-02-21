using EtlApp.Adapter.Csv;
using EtlApp.Adapter.Sql;
using EtlApp.Domain.Config;
using EtlApp.Domain.Execution;
using EtlApp.Domain.Middleware;
using EtlApp.Domain.Module;
using EtlApp.Domain.Source;

MiddlewareManager middlewareManager = new MiddlewareManager();
middlewareManager.Register("type_inference", new DetectTypeMiddleware());
middlewareManager.Register("type_cast", new CastTypeMiddleware());
middlewareManager.Register("type_validation", new ValidateTypeMiddleware());

ModuleRegistry moduleRegistry = new ModuleRegistry();
ExecutionService executionService = new ExecutionService(moduleRegistry);
moduleRegistry.RegisterModule(new CsvModule());
moduleRegistry.RegisterModule(new SqlModule());

var pipeline = executionService.Build(
    new CsvSourceConfig
    {
        Delimiter = ",",
        FilePath = "C:\\Users\\alexa\\RiderProjects\\etl-tool\\EtlApp\\TestFiles",
        FilePrefix = "te",
        Type = "csv"
    },
    new SqlTargetConfig
    {
        Type = "sql",
        ConnectionName = "sql",
        TableName = "test",
    },
    new MappingConfig
    {
        Mappings = []
    },
    [
        middlewareManager.Get("type_inference")!, 
        middlewareManager.Get("type_cast")!,
        middlewareManager.Get("type_validation")!
    ]
);

pipeline.Run();