using System.Data;
using EtlApp.Domain.Config;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Dto;
using Microsoft.Extensions.Logging;

namespace EtlApp.Adapter.BuildIn.Middlewares;

public class DetectTypeMiddlewareConfig : MiddlewareConfig
{
    public bool OnlyUnknown { get; set; } = true;
}

public class DetectTypeMiddleware(DetectTypeMiddlewareConfig config, PipelineContext context)
    : Domain.Connection.Middleware
{
    private readonly ILogger _logger = Logging.LoggerFactory.CreateLogger<DetectTypeMiddleware>();

    protected override ReportData Process(ReportData reportData)
    {
        foreach (var (columnName, columnConfig) in reportData.Columns)
        {
            if (config.OnlyUnknown && columnConfig.SourceType != ColumnType.Undefined)
            {
                continue;
            }

            var detectedType = DetectColumnType(reportData.Data, columnName);
            _logger.LogDebug($"Detected type: {columnName}: {detectedType}");
            columnConfig.SourceType = detectedType;
        }

        _logger.LogDebug("Detecting types {}", reportData.Columns.Values
            .Select(x => $"({x.SourceName}:{x.SourceType})"));

        return reportData;
    }

    private ColumnType DetectColumnType(DataTable dataTable, string columnName)
    {
        bool allInts = true, allDoubles = true, allDates = true, allDateTimes = true, allBool = true;

        foreach (DataRow row in dataTable.Rows)
        {
            var value = row[columnName];
            if (string.IsNullOrWhiteSpace(value.ToString())) continue;

            // Check for each type
            if (!TryParseInt(value)) allInts = false;
            if (!TryParseDouble(value)) allDoubles = false;
            if (!TryParseDate(value)) allDates = false;
            if (!TryParseDateTime(value)) allDateTimes = false;
            if (!TryParseBool(value)) allBool = false;

            // Exit early if all checks fail
            if (!allInts && !allDoubles && !allDates && !allDateTimes && !allBool)
                break;
        }

        if (allInts) return ColumnType.Int;
        if (allDoubles) return ColumnType.Decimal;
        if (allDates) return ColumnType.Date;
        if (allDateTimes) return ColumnType.DateTime;
        if (allBool) return ColumnType.Boolean;

        return ColumnType.String;
    }

    private bool TryParseInt(object value)
    {
        return value is int || value is long || (value is string str && int.TryParse(str, out _));
    }

    private bool TryParseDouble(object value)
    {
        return value is double || (value is string str && double.TryParse(str, out _));
    }

    private bool TryParseDateTime(object value)
    {
        return value is DateTime || (value is string str && DateTime.TryParse(str, out _));
    }

    private bool TryParseDate(object value)
    {
        return value is DateOnly || (value is string str && DateOnly.TryParse(str, out _));
    }

    private bool TryParseBool(object value)
    {
        return value is bool || (value is string str && bool.TryParse(str, out _));
    }
}