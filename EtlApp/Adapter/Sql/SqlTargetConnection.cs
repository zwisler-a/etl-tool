using System.Data.Common;
using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Target;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using static EtlApp.Adapter.Sql.SqlCommands;

namespace EtlApp.Adapter.Sql;

public class SqlTargetConnection : ITargetConnection
{
    private readonly ILogger _logger = Logging.LoggerFactory.CreateLogger<SqlTargetConnection>();
    private readonly SqlTargetConfig _config;
    private readonly PipelineContext _context;
    private readonly DbConnection _dbConnection;

    public SqlTargetConnection(SqlTargetConfig config, PipelineContext context)
    {
        _config = config;
        _context = context;

        var con = _context.DatabaseManager.GetConnection(_config.ConnectionName) ??
                  throw new Exception("Unknown connection");
        _dbConnection = new SqlConnection(con.ConnectionString);
    }


    public List<UpdateStrategy> GetSupportedUpdateStrategies()
    {
        return
        [
            UpdateStrategy.ReplaceComplete,
            UpdateStrategy.ArchiveInTable,
            UpdateStrategy.MergeByUnique,
            UpdateStrategy.Append
        ];
    }

    public void OnCompleted()
    {
        _dbConnection.Close();
    }

    public void OnError(Exception error)
    {
        _dbConnection.Close();
    }

    public void OnNext(ReportData report)
    {
        if (report.Data.Rows.Count == 0) return; // Sanity check
        _dbConnection.Open();
        switch (_config.UpdateStrategy)
        {
            case UpdateStrategy.ReplaceComplete:
                CreateTableIfNecessary(report, _dbConnection, _config.TableName);
                ClearTable(_dbConnection, _config.TableName);
                UploadIntoTable(report, _dbConnection, _config.TableName);
                break;
            case UpdateStrategy.ArchiveInTable:
                var archiveTable = _config.ArchiveTableName ?? $"{_config.TableName}_Archive";
                CreateTableIfNecessary(report, _dbConnection, _config.TableName);
                CreateTableIfNecessary(report, _dbConnection, archiveTable);
                CopyTable(_dbConnection, _config.TableName, archiveTable);
                ClearTable(_dbConnection, _config.TableName);
                UploadIntoTable(report, _dbConnection, _config.TableName);
                break;
            case UpdateStrategy.Append:
                CreateTableIfNecessary(report, _dbConnection, _config.TableName);
                UploadIntoTable(report, _dbConnection, _config.TableName);
                break;
            case UpdateStrategy.MergeByUnique:
                var staging = _config.StagingTableName ?? $"Staging_{_config.TableName}";
                CreateTableIfNecessary(report, _dbConnection, _config.TableName);
                CreateTableIfNecessary(report, _dbConnection, staging);
                ClearTable(_dbConnection, staging);
                UploadIntoTable(report, _dbConnection, staging);
                var mergeKeys = GetMergeKeys(report);
                MergeTables(_dbConnection, staging, _config.TableName, mergeKeys.ToArray());
                break;
        }

        _dbConnection.Close();
    }

    private static List<string> GetMergeKeys(ReportData report)
    {
        var mergeKeys = new List<string>();
        foreach (var (key, value) in report.Columns)
        {
            var val = value.Modifiers;
            if (val == null) continue;
            if (val.Contains(ColumnModifiers.DuplicateIdentifier) || val.Contains(ColumnModifiers.PrimaryKey))
            {
                mergeKeys.Add(key);
            }
        }

        return mergeKeys;
    }
}