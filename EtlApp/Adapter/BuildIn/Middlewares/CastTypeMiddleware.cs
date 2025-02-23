using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using EtlApp.Domain.Config;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Dto;
using Microsoft.Extensions.Logging;


namespace EtlApp.Adapter.BuildIn.Middlewares;

public class CastTypeMiddlewareConfig : MiddlewareConfig
{
    public CultureInfo? CultureInfo { get; set; }
}

public class CastTypeMiddleware(CastTypeMiddlewareConfig config, PipelineContext context) : Domain.Connection.Middleware
{
    private readonly ILogger _logger = Logging.LoggerFactory.CreateLogger<CastTypeMiddleware>();

    protected override ReportData Process(ReportData reportData)
    {
        var newDataTable = new DataTable();
        var columns = reportData.Columns.ToList();
    
        foreach (var (columnName, columnConfig) in columns)
            newDataTable.Columns.Add(columnName, GetColumnType(columnConfig.SourceType));
    
        newDataTable.BeginLoadData();
        foreach (DataRow originalRow in reportData.Data.Rows)
        {
            var newRow = newDataTable.NewRow();
            foreach (var (columnName, columnConfig) in columns)
                newRow[columnName] = ConvertValue(originalRow[columnName], columnConfig.SourceType);
            newDataTable.Rows.Add(newRow);
        }
    
        newDataTable.EndLoadData();
        reportData.Data.Clear();
        return reportData with { Data = newDataTable };
    }
    
    private Type GetColumnType(ColumnType targetType) => targetType switch
    {
        ColumnType.Int => typeof(int),
        ColumnType.Decimal => typeof(double),
        ColumnType.DateTime => typeof(DateTime),
        ColumnType.Date => typeof(DateOnly),
        ColumnType.String => typeof(string),
        ColumnType.Boolean => typeof(bool),
        _ => throw new Exception($"Unsupported ColumnType {targetType}")
    };

    private object ConvertValue(object value, ColumnType targetType)
    {
        if (string.IsNullOrWhiteSpace(value.ToString())) return DBNull.Value;
        return targetType switch
        {
            ColumnType.Int => TryConvertInt(value),
            ColumnType.Decimal => TryConvertDecimal(value),
            ColumnType.DateTime => value is DateTime ? value : Convert.ToDateTime(value),
            ColumnType.Date => value is DateOnly ? value :
                value is DateTime dt ? DateOnly.FromDateTime(dt) : DateOnly.Parse(value.ToString()),
            ColumnType.Boolean => Convert.ToBoolean(value),
            ColumnType.String => value.ToString(),
            _ => throw new Exception($"Unsupported ColumnType {targetType}")
        };
    }

    private object? TryConvertInt(object? value)
    {
        if (value is null) return value;
        if (value is int intValue) return intValue;
        if (int.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int ival))
            return ival;

        throw new FormatException($"Could not convert value '{value}' to an integer.");
    }

    private object? TryConvertDecimal(object? value)
    {
        if (value is null) return value;
        if (value is float f) return f;
        if (value is decimal d) return d;
        if (value is double dou) return dou;

        string stringValue = value.ToString()!;
        CultureInfo culture;

        if (config.CultureInfo != null)
        {
            return Convert.ToDouble(value, config.CultureInfo);
        }

        bool containsComma = stringValue.Contains(",");
        bool containsDot = stringValue.Contains(".");
        if (containsComma && containsDot)
        {
            _logger.LogError("Ambiguous decimal separator, please specify culture!");
            throw new FormatException("Ambiguous decimal separator.");
        }

        if (containsComma)
        {
            culture = CultureInfo.GetCultureInfo("de-DE");
        }
        else
        {
            culture = CultureInfo.InvariantCulture;
        }

        if (double.TryParse(stringValue, NumberStyles.Number, culture, out var doubleResult))
            return doubleResult;

        throw new FormatException($"Could not convert value '{stringValue}' to an decimal.");
    }
}