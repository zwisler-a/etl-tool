using System.Data.Common;
using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EtlApp.Adapter.Sql;

public static class SqlCommands
{
    private static ILogger _logger = Logging.LoggerFactory.CreateLogger("SqlCommands");

    public static void ClearTable(DbConnection dbConnection, string tableName)
    {
        var clearTableCommandText = $"DELETE FROM {tableName}";
        _logger.LogDebug("Clear Table SQL: \"{}\"", clearTableCommandText);

        using var clearTableCommand = new SqlCommand(clearTableCommandText, (SqlConnection)dbConnection);
        clearTableCommand.ExecuteNonQuery();
    }

    public static void MergeTables(DbConnection dbConnection, string sourceTable, string destinationTable,
        string[] mergeKeyColumns)
    {
        var getColumnsCommandText =
            $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{sourceTable}'";
        var columns = new List<string>();

        using (var getColumnsCommand = new SqlCommand(getColumnsCommandText, (SqlConnection)dbConnection))
        using (var reader = getColumnsCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                columns.Add(reader.GetString(0));
            }
        }

        // Construct the ON clause using multiple columns for matching
        var onClause = string.Join(" AND ", mergeKeyColumns.Select(col => $"target.{col} = source.{col}"));

        // Construct the SET clause for the UPDATE (updates all columns except the merge keys)
        var updateSet = string.Join(", ", columns.Where(col => !mergeKeyColumns.Contains(col))
            .Select(col => $"target.{col} = source.{col}"));

        // Construct the column list for the INSERT clause (inserts all columns)
        var insertColumns = string.Join(", ", columns);
        var insertValues = string.Join(", ", columns.Select(col => $"source.{col}"));

        // SQL to merge data from source table into destination table
        var mergeTableCommandText = $@"
        MERGE INTO {destinationTable} AS target
        USING {sourceTable} AS source
        ON {onClause}
        WHEN MATCHED THEN
            UPDATE SET {updateSet}
        WHEN NOT MATCHED BY TARGET THEN
            INSERT ({insertColumns})
            VALUES ({insertValues});";

        _logger.LogDebug("Merge Tables SQL: \"{}\"", mergeTableCommandText);

        using (var mergeTableCommand = new SqlCommand(mergeTableCommandText, (SqlConnection)dbConnection))
        {
            mergeTableCommand.ExecuteNonQuery();
        }
    }


    public static void CopyTable(DbConnection dbConnection, string sourceTable, string destinationTable)
    {
        var copyTableCommandText = $"INSERT INTO {destinationTable} SELECT * FROM {sourceTable}";

        _logger.LogDebug("Copy Table SQL: \"{}\"", copyTableCommandText);

        using var copyTableCommand = new SqlCommand(copyTableCommandText, (SqlConnection)dbConnection);
        copyTableCommand.ExecuteNonQuery();
    }

    public static void CreateTableIfNecessary(ReportData report, DbConnection dbConnection, string tableName)
    {
        using var transaction = (SqlTransaction)dbConnection.BeginTransaction();

        // Check if the table exists, create it if necessary
        var checkTableCommandText = CreateTableDefinition(report, tableName);
        _logger.LogDebug("Check Table SQL: \"{}\"", checkTableCommandText);

        using (var checkTableCommand = new SqlCommand(checkTableCommandText, (SqlConnection)dbConnection))
        {
            checkTableCommand.Transaction = transaction;
            checkTableCommand.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    public static void UploadIntoTable(ReportData report, DbConnection dbConnection, string tableName)
    {
        var dataTable = report.Data; // Assuming report.Data is a DataTable

        using var bulkCopy = new SqlBulkCopy((SqlConnection)dbConnection);
        bulkCopy.DestinationTableName = tableName;
        bulkCopy.BatchSize = 1000;
        bulkCopy.NotifyAfter = 1000;

        foreach (var columnName in report.Columns.Keys)
        {
            bulkCopy.ColumnMappings.Add(columnName, columnName); // Map columns
        }

        var totalRows = report.Data.Rows.Count;
        long rowsProcessed = 0;

        bulkCopy.SqlRowsCopied += (sender, args) =>
        {
            rowsProcessed = args.RowsCopied;
            if (rowsProcessed % 100000 != 0) return;
            var progressPercentage = (int)((double)rowsProcessed / totalRows * 100);
            _logger.LogDebug($"Progress: {progressPercentage}%");
        };

        bulkCopy.WriteToServer(dataTable);
        bulkCopy.Close();
    }


    private static string CreateTableDefinition(ReportData data, string tableName)
    {
        var typeDefinition = data.Columns.Select((c) =>
        {
            var (_, mappingConfig) = c;
            var sqlType = SqlTypes.ColumnTypeToSqlType.GetValueOrDefault(mappingConfig.SourceType) ?? "varchar(255)";
            var modifiers = mappingConfig.Modifiers?.Select(modifier =>
            {
                return modifier switch
                {
                    ColumnModifiers.PrimaryKey => "PRIMARY KEY",
                    ColumnModifiers.Unique => "UNIQUE",
                    _ => ""
                };
            }) ?? [""];
            return $"{mappingConfig.TargetName} {sqlType} {string.Join(" ", modifiers!)}";
        });
        var tableDefinitionBody = string.Join(", ", typeDefinition);

        return
            $"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}') " +
            $"CREATE TABLE {tableName} ({tableDefinitionBody})";
    }
}