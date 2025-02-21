using System.Data;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;

namespace EtlApp.Adapter.Csv;

public class CsvTargetConnection(CsvTargetConfig config) : ITargetConnection
{
    public void Upload(ReportData report, PipelineExecutionContext context)
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = config.Delimiter,
        };
    
        var fileName = Path.Combine(config.FilePath, $"{config.FilePrefix}_{DateTime.UtcNow:yyyyMMddHHmmssfff}.csv");
    
        using var writer = new StreamWriter(fileName);
        using var csv = new CsvWriter(writer, csvConfig);
    
        // Write header
        foreach (DataColumn column in report.Data.Columns)
        {
            csv.WriteField(column.ColumnName);
        }
        csv.NextRecord();
    
        // Write data
        foreach (DataRow row in report.Data.Rows)
        {
            foreach (DataColumn column in report.Data.Columns)
            {
                csv.WriteField(row[column]);
            }
            csv.NextRecord();
        }
    }
}