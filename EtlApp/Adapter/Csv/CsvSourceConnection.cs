using System.Data;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Source;

namespace EtlApp.Adapter.Csv;

public class CsvSourceConnection(CsvSourceConfig config) : ISourceConnection
{
    public List<ReportData> Fetch()
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = config.Delimiter,
        };

        var files = Directory.GetFiles(config.FilePath, $"{config.FilePrefix}*.csv");
        var reportDataList = new List<ReportData>();

        foreach (var file in files)
        {
            using var reader = new StreamReader(file);
            using var csv = new CsvReader(reader, csvConfig);
            using var dataReader = new CsvDataReader(csv);

            var dt = new DataTable();
            dt.Load(dataReader);
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
            var report = new ReportData(dt, columnNames);
            reportDataList.Add(report);
        }

        return reportDataList;
    }
}