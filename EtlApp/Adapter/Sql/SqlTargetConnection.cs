using System.Data;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using EtlApp.Domain.Target;
using Microsoft.Data.SqlClient;

namespace EtlApp.Adapter.Sql;

public class SqlTargetConnection(SqlTargetConfig config) : ITargetConnection
{
    public void Upload(ReportData data, PipelineExecutionContext context)
    {
        var dbConnection = context.DatabaseManager.GetConnection(config.ConnectionName) ??
                           throw new Exception("Unknown connection");

        dbConnection.Open();
        using var transaction = (SqlTransaction)dbConnection.BeginTransaction();

        // Check if the table exists, create it if necessary
        var checkTableCommandText =
            $"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{config.TableName}') " +
            $"CREATE TABLE {config.TableName} ({string.Join(", ", data.ColumnNames.Select(c => $"{c} NVARCHAR(MAX)"))})";

        using (var checkTableCommand = new SqlCommand(checkTableCommandText, (SqlConnection)dbConnection))
        {
            checkTableCommand.Transaction = transaction;
            checkTableCommand.ExecuteNonQuery();
        }

        if (data.ColumnNames != null && data.Data.Rows.Count > 0)
        {
            var columns = string.Join(", ", data.ColumnNames);
            var values = string.Join(", ", data.ColumnNames.Select(c => "@" + c));

            var commandText = $"INSERT INTO {config.TableName} ({columns}) VALUES ({values})";

            using var command = new SqlCommand(commandText, (SqlConnection)dbConnection);
            foreach (var columnName in data.ColumnNames)
            {
                command.Parameters.AddWithValue("@" + columnName, DBNull.Value); // Handle proper value assignment
            }

            foreach (DataRow row in data.Data.Rows)
            {
                foreach (var columnName in data.ColumnNames)
                {
                    command.Parameters[$"@{columnName}"].Value = row[columnName] ?? DBNull.Value;
                }

                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }
        }

        transaction.Commit();
    }
}