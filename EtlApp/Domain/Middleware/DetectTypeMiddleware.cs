using System.Data;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Middleware;

public class DetectTypeMiddleware : IMiddleware
{
    private readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger<DetectTypeMiddleware>();

    public ReportData Process(ReportData reportData, PipelineExecutionContext context)
    {
        foreach (var (columnName, columnConfig) in reportData.Columns)
        {
            var columnValues = reportData.Data.AsEnumerable()
                .Select(row => row[columnName])
                .ToList();

            var detectedType = DetectColumnType(columnValues);
            _logger.LogInformation($"Detected type: {columnName}: {detectedType}");
            columnConfig.SourceType = detectedType;
        }

        return reportData;
    }


    private ColumnType DetectColumnType(List<object> columnValues)
    {
        if (columnValues.All(value => TryParseInt(value)))
            return ColumnType.Int;
        if (columnValues.All(value => TryParseDouble(value)))
            return ColumnType.Decimal;
        if (columnValues.All(value => TryParseDateTime(value)))
            return ColumnType.DateTime;

        return ColumnType.String;
    }

    private bool TryParseInt(object value)
    {
        return value is int || (value is string str && int.TryParse(str, out _));
    }

    private bool TryParseDouble(object value)
    {
        return value is double || (value is string str && double.TryParse(str, out _));
    }

    private bool TryParseDateTime(object value)
    {
        return value is DateTime || (value is string str && DateTime.TryParse(str, out _));
    }
}