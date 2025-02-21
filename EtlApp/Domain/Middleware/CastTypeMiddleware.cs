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
        var newDataTable = reportData.Data.Clone(); // Clone the structure of the original DataTable

        // Remove the ReadOnly property from each column in the new DataTable
        foreach (DataColumn column in newDataTable.Columns)
        {
            column.ReadOnly = false;
        }

        // Copy the rows from the original DataTable to the new DataTable
        foreach (DataRow originalRow in reportData.Data.Rows)
        {
            newDataTable.ImportRow(originalRow);
        }

        foreach (var (columnName, columnConfig) in reportData.Columns)
        {
            var columnValues = reportData.Data.AsEnumerable()
                .Select(row => row[columnName])
                .ToList();

            var castedValues = CastColumnType(columnValues, columnConfig.SourceType);

            for (var i = 0; i < columnValues.Count; i++)
            {
                newDataTable.Rows[i][columnName] = castedValues[i];
            }
        }

        // Create a new ReportData with the new DataTable
        return reportData with { Data = newDataTable };
    }


    private List<object> CastColumnType(List<object> columnValues, ColumnType targetType)
    {
        return targetType switch
        {
            ColumnType.Int => columnValues.Select(value => Convert.ToInt32(value)).ToList(),
            ColumnType.Decimal => columnValues.Select(value => value is double ? value : Convert.ToDouble(value))
                .ToList(),
            ColumnType.DateTime => columnValues.Select(value => value is DateTime ? value : Convert.ToDateTime(value))
                .ToList(),
            ColumnType.String => columnValues.Select(value => value).ToList(),
            _ => throw new Exception($"Unsupported ColumnType {targetType}")
        };
    }
}