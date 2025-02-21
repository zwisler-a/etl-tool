using System.Data;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Middleware;

public class ValidateTypeMiddleware : IMiddleware
{
    private readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger<ValidateTypeMiddleware>();

    public ReportData Process(ReportData reportData, PipelineExecutionContext context)
    {
        foreach (var (columnName, columnConfig) in reportData.Columns)
        {
            var columnValues = reportData.Data.AsEnumerable()
                .Select(row => row[columnName])
                .ToList();

            var isValid = ValidateColumnType(columnValues, columnConfig.SourceType);
            if (!isValid)
            {
                _logger.LogWarning($"Validation failed for column '{columnName}' with type {columnConfig.SourceType}-{columnValues}");
                throw new Exception($"Validation failed for column '{columnName}' with type {columnConfig.SourceType}");
            }
        }

        return reportData;
    }

    private bool ValidateColumnType(List<object> columnValues, ColumnType expectedType)
    {
        return expectedType switch
        {
            ColumnType.Int => columnValues.All(value => value is int),
            ColumnType.Decimal => columnValues.All(value => value is double),
            ColumnType.DateTime => columnValues.All(value => value is DateTime),
            ColumnType.String => columnValues.All(value => value is string),
            _ => false
        };
    }
}