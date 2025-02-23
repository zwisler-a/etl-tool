using System.Data;
using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Util.Observable;

namespace EtlApp.Adapter.Csv;

public class CsvSourceConnection(CsvSourceConfig config, PipelineContext context)
    : Observable<ReportData>, ISourceConnection
{
    public void Fetch()
    {
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = config.Delimiter,
        };

        var files = Directory.GetFiles(config.FilePath, $"{config.FilePrefix}*.csv");

        foreach (var file in files)
        {
            try
            {
                using var reader = new StreamReader(file);
                using var csv = new CsvReader(reader, csvConfig);
                csv.Context.TypeConverterCache.AddConverter<DateTime>(
                    new CustomDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
                csv.Context.TypeConverterCache.AddConverter<DateOnly>(new CustomDateOnlyConverter("yyyy-MM-dd"));

                var records = csv.GetRecords<dynamic>();
                var dt = new DataTable();
                var columnTypes = new Dictionary<string, ColumnMappingConfig>();
                bool initialized = false;
                int rowCount = 0;
                int batchSize = config.BatchSize;

                foreach (var record in records)
                {
                    if (!initialized)
                    {
                        foreach (var kvp in record)
                        {
                            dt.Columns.Add(kvp.Key);
                        }

                        columnTypes = dt.Columns.Cast<DataColumn>()
                            .ToDictionary(c => c.ColumnName, c => GetColumnType(c, context));
                        initialized = true;
                    }

                    var row = dt.NewRow();
                    foreach (var kvp in record)
                    {
                        row[kvp.Key] = kvp.Value;
                    }

                    dt.Rows.Add(row);
                    rowCount++;

                    if (batchSize > 0 && rowCount >= batchSize)
                    {
                        var report = new ReportData(dt, columnTypes);
                        Next(report);
                        dt.Clear();
                        rowCount = 0;
                    }
                }

                if (rowCount > 0)
                {
                    var report = new ReportData(dt, columnTypes);

                    Next(report);
                }
                reader.Close();
                if (config.MoveToArchive)
                {
                    MoveFileToSuccessFullArchive(file);
                }
            }
            catch (Exception)
            {
                if (config.MoveToArchive)
                {
                    MoveFileToFailedFullArchive(file);
                }

                throw;
            }
        }

        Complete();
    }

    private void MoveFileToFailedFullArchive(string file)
    {
        Debug.Assert(config.FailedArchivePath != null);
        var failedArchivePath = Path.Combine(config.FailedArchivePath, Path.GetFileName(file));
        Directory.CreateDirectory(config.FailedArchivePath);
        File.Move(file, failedArchivePath, true);
    }

    private void MoveFileToSuccessFullArchive(string file)
    {
        Debug.Assert(config.SuccessArchivePath != null);
        var successArchivePath = Path.Combine(config.SuccessArchivePath, Path.GetFileName(file));
        Directory.CreateDirectory(config.SuccessArchivePath);
        File.Move(file, successArchivePath, true);
    }

    private static ColumnMappingConfig GetColumnType(DataColumn column, PipelineContext context)
    {
        var source =
            context.MappingConfig.Mappings.Find(mappingConfig => mappingConfig.SourceName.Equals(column.ColumnName));
        return source ?? new ColumnMappingConfig
            { SourceName = column.ColumnName, SourceType = ColumnType.Undefined, TargetName = column.ColumnName };
    }
}