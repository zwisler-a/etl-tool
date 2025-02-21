using EtlApp.Adapter.Csv;
using EtlApp.Adapter.Sql;
using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using EtlApp.Domain.Module;
using EtlApp.Domain.Source;


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
        Mappings =
        [
            new PropertyMappingConfig { SourceName = "col1", TargetName = "Column1", SourceType = ColumnType.Int },
            new PropertyMappingConfig { SourceName = "col2", TargetName = "Column2", SourceType = ColumnType.Int },
            new PropertyMappingConfig { SourceName = "col3", TargetName = "Column3", SourceType = ColumnType.Int },
            new PropertyMappingConfig { SourceName = "col4", TargetName = "Column4", SourceType = ColumnType.String }
        ]
    }
);

pipeline.Run();