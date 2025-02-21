using System.Data;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Middleware;

public class CastTypeMiddleware : IMiddleware
{
    private readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger<CastTypeMiddleware>();

    public ReportData Process(ReportData reportData, PipelineExecutionContext context)
    {
        var newDataTable = new DataTable();

        // Adjust column data types
        foreach (var (columnName, columnConfig) in reportData.Columns)
        {
            var columnType = GetColumnType(columnConfig.SourceType);
            newDataTable.Columns.Add(columnName, columnType);
        }

        // Copy data with type casting
        foreach (DataRow originalRow in reportData.Data.Rows)
        {
            var newRow = newDataTable.NewRow();
            foreach (var (columnName, columnConfig) in reportData.Columns)
            {
                newRow[columnName] = ConvertValue(originalRow[columnName], columnConfig.SourceType);
            }

            newDataTable.Rows.Add(newRow);
        }

        return reportData with { Data = newDataTable };
    }

    private Type GetColumnType(ColumnType targetType) => targetType switch
    {
        ColumnType.Int => typeof(int),
        ColumnType.Decimal => typeof(double),
        ColumnType.DateTime => typeof(DateTime),
        ColumnType.String => typeof(string),
        _ => throw new Exception($"Unsupported ColumnType {targetType}")
    };

    private object ConvertValue(object value, ColumnType targetType) => (targetType switch
    {
        ColumnType.Int => Convert.ToInt32(value),
        ColumnType.Decimal => value is double ? value : Convert.ToDouble(value),
        ColumnType.DateTime => value is DateTime ? value : Convert.ToDateTime(value),
        ColumnType.String => value.ToString(),
        _ => throw new Exception($"Unsupported ColumnType {targetType}")
    })!;
}