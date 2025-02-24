using System.Data;
using EtlApp.Domain.Config;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Util.Observable;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EtlApp.Adapter.Sql;

public class SqlSourceConnection(SqlSourceConfig config, PipelineContext context)
    : Observable<ReportData>, ISourceConnection
{
    private readonly ILogger _logger = Logging.LoggerFactory.CreateLogger<SqlSourceConnection>();

    public void Fetch()
    {
        var dbConnection = context.DatabaseManager.GetConnection(config.ConnectionName) ??
                           throw new Exception("Unknown connection");
        dbConnection.Open();

        var commandText = CreateSelectStatement();
        _logger.LogDebug("SQL Select statement: \"{}\"", commandText);

        using var command = new SqlCommand(commandText, (SqlConnection)dbConnection);

        using var reader = command.ExecuteReader();
        var reportData = new ReportData(new DataTable(), new Dictionary<string, ColumnMappingConfig>());

        for (var i = 0; i < reader.FieldCount; i++)
        {
            var columnName = reader.GetName(i);
            reportData.Columns[columnName] = context.GetColumnMapping(columnName,
                SqlTypes.DataTypeToSqlType.GetValueOrDefault(reader.GetFieldType(i)!));
            reportData.Data.Columns.Add(columnName);
        }

        while (reader.Read())
        {
            var row = reportData.Data.NewRow();
            foreach (var column in reportData.Columns.Keys)
            {
                row[column] = reader[column];
            }

            reportData.Data.Rows.Add(row);
            if (config.BatchSize > 0 && config.BatchSize == reportData.Data.Rows.Count)
            {
                Next(reportData);
                reportData = new ReportData(reportData.Data.Clone(), new(reportData.Columns));
            }
        }

        if (reportData.Data.Rows.Count != 0)
        {
            Next(reportData);
        }

        dbConnection.Close();
        Complete();
    }

    private string CreateSelectStatement()
    {
        return $"SELECT * FROM {config.TableName}";
    }
}