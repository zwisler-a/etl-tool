using System.Data;
using EtlApp.Domain.Config;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Dto;
using Microsoft.Extensions.Logging;

namespace EtlApp.Adapter.BuildIn.Middlewares;

public class ValidateTypeMiddlewareConfig : MiddlewareConfig;

public class ValidateTypeMiddleware(ValidateTypeMiddlewareConfig config, PipelineContext context)
    : Domain.Connection.Middleware
{
    private readonly ILogger _logger = Logging.LoggerFactory.CreateLogger<ValidateTypeMiddleware>();

    protected override ReportData Process(ReportData reportData)
    {
        foreach (var (columnName, columnConfig) in reportData.Columns)
        {
            var isValid = true;

            foreach (var row in reportData.Data.AsEnumerable())
            {
                var value = row[columnName];
                if (!ValidateColumnType(value, columnConfig))
                {
                    isValid = false;
                    _logger.LogWarning(
                        $"Validation failed for column '{columnName}' with type {columnConfig.SourceType} on value \"{value}\"");
                    break;
                }
            }

            if (!isValid)
            {
                throw new Exception($"Validation failed for column '{columnName}' with type {columnConfig.SourceType}");
            }
        }

        return reportData;
    }

    private bool ValidateColumnType(object value, ColumnMappingConfig config)
    {
        var failNullValues = (config.Modifiers?.Contains(ColumnModifiers.PrimaryKey) ?? false) ||
                             (config.Modifiers?.Contains(ColumnModifiers.NonNull) ?? false);
        if (failNullValues && value is DBNull) return false;
        if (value is DBNull) return true;
        return config.SourceType switch
        {
            ColumnType.Int => value is int,
            ColumnType.Decimal => value is double,
            ColumnType.Date => value is DateOnly,
            ColumnType.DateTime => value is DateTime,
            ColumnType.String => value is string,
            ColumnType.Boolean => value is bool,
            _ => false
        };
    }
}