using EtlApp.Domain.Config;
using EtlApp.Domain.Database;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Execution;

public record PipelineExecutionContext(
    DatabaseManager DatabaseManager,
    ILogger Logger,
    MappingConfig MappingConfig
);