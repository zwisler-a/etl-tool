using System.Data;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;
using Microsoft.Extensions.Logging;

namespace EtlApp.Adapter.Csv;

public class CsvTargetConnection : ITargetConnection
{
    private readonly CsvTargetConfig _config;
    private readonly StreamWriter _writer;
    private readonly CsvWriter _csv;
    private bool _firstCall = true;

    public List<UpdateStrategy> GetSupportedUpdateStrategies()
    {
        return [UpdateStrategy.ReplaceComplete];
    }

    public CsvTargetConnection(CsvTargetConfig config)
    {
        _config = config;
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = config.Delimiter,
        };

        var fileName = Path.Combine(config.FilePath, $"{config.FilePrefix}_{DateTime.UtcNow:yyyyMMddHHmmssfff}.csv");

        _writer = new StreamWriter(fileName);
        _csv = new CsvWriter(_writer, csvConfig);
        _csv.Context.TypeConverterCache.AddConverter<DateTime>(new CustomDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
        _csv.Context.TypeConverterCache.AddConverter<DateOnly>(new CustomDateOnlyConverter("yyyy-MM-dd"));
    }
    
    
    public void OnNext(ReportData report)
    {
        if (_firstCall)
        {
            foreach (DataColumn column in report.Data.Columns)
            {
                _csv.WriteField(column.ColumnName);
            }
            _csv.NextRecord();
            _firstCall = false;
        }

        // Write data
        foreach (DataRow row in report.Data.Rows)
        {
            foreach (DataColumn column in report.Data.Columns)
            {
                _csv.WriteField(row[column]);
            }

            _csv.NextRecord();
        }
        _csv.Flush();
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }
    
}