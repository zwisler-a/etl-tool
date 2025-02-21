using System.Data;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using EtlApp.Domain.Target;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EtlApp.Adapter.Sql;

public class SqlTargetConnection(SqlTargetConfig config) : ITargetConnection
{
    private readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger<SqlTargetConnection>();

    public void Upload(ReportData data, PipelineExecutionContext context)
    {
        var dbConnection = context.DatabaseManager.GetConnection(config.ConnectionName) ??
                           throw new Exception("Unknown connection");

        dbConnection.Open();
        using var transaction = (SqlTransaction)dbConnection.BeginTransaction();

        // Check if the table exists, create it if necessary
        var checkTableCommandText = CreateTableDefinition(data);
        _logger.LogInformation("Check Table SQL {}", checkTableCommandText);

        using (var checkTableCommand = new SqlCommand(checkTableCommandText, (SqlConnection)dbConnection))
        {
            checkTableCommand.Transaction = transaction;
            checkTableCommand.ExecuteNonQuery();
        }

        if (data.Data.Rows.Count > 0)
        {
            var commandText = CreateInsertStatement(data);
            _logger.LogInformation("SQL Insert statement {}", commandText);

            using var command = new SqlCommand(commandText, (SqlConnection)dbConnection);
            foreach (var columnName in data.Columns.Keys)
            {
                command.Parameters.AddWithValue("@" + columnName, DBNull.Value); // Handle proper value assignment
            }

            foreach (DataRow row in data.Data.Rows)
            {
                foreach (var columnName in data.Columns.Keys)
                {
                    command.Parameters[$"@{columnName}"].Value = row[columnName] ?? DBNull.Value;
                }

                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }
        }

        transaction.Commit();
    }

    private string CreateTableDefinition(ReportData data)
    {
        var typeDefinition = data.Columns.Select((c) =>
        {
            var (_, mappingConfig) = c;
            var sqlType = SqlTypes.DataTypes.GetValueOrDefault(mappingConfig.SourceType) ?? "varchar(255)";
            return $"{mappingConfig.TargetName} {sqlType}";
        });
        var tableDefinitionBody = string.Join(", ", typeDefinition);

        return
            $"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{config.TableName}') " +
            $"CREATE TABLE {config.TableName} ({tableDefinitionBody})";
    }

    private string CreateInsertStatement(ReportData data)
    {
        var columns = data.Columns.Select(c =>
        {
            var (_, mappingConfig) = c;
            return $"{mappingConfig.TargetName}";
        });

        var values = data.Columns.Select(c =>
        {
            var (_, mappingConfig) = c;
            return $"@{mappingConfig.SourceName}";
        });

        return $"INSERT INTO {config.TableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";
    }
}