using System.Data;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using EtlApp.Domain.Source;

namespace EtlApp.Adapter.Csv;

public class CsvSourceConnection(CsvSourceConfig config) : ISourceConnection
{
    public List<ReportData> Fetch(PipelineExecutionContext context)
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
            var columnTypes = dt.Columns.Cast<DataColumn>()
                .ToDictionary(c => c.ColumnName, c => GetColumnType(c, context));

            var report = new ReportData(dt, columnTypes);
            reportDataList.Add(report);
        }

        return reportDataList;
    }

    private static ColumnType GetColumnType(DataColumn column, PipelineExecutionContext context)
    {
        var source =
            context.MappingConfig.Mappings.Find(mappingConfig => mappingConfig.SourceName.Equals(column.ColumnName));
        return source?.SourceType ?? ColumnType.String;
    }
}