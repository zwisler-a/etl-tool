using System.Data;
using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Middleware;

public class TransformMiddleware : IMiddleware
{
    private readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger<TransformMiddleware>();

    public ReportData Process(ReportData reportData, PipelineExecutionContext context)
    {
        var newDataTable = new DataTable();
        
        // Copy data with type casting
        foreach (DataRow originalRow in reportData.Data.Rows)
        {
            var newRow = newDataTable.NewRow();
            foreach (var (columnName, columnConfig) in reportData.Columns)
            {
                newRow[columnName] = ConvertValue(originalRow[columnName], columnConfig);
            }

            newDataTable.Rows.Add(newRow);
        }

        return reportData with { Data = newDataTable };
    }


    private object ConvertValue(object value, ColumnMappingConfig mapping)
    {
        return null;
    }


    private class Globals
    {
        public string input { get; set; }
    }
    
    static string ExecuteCode(string code, string input)
    {
        var scriptOptions = ScriptOptions.Default
            .WithReferences(AppDomain.CurrentDomain.GetAssemblies()) // Add necessary references
            .WithImports("System");

        var script = CSharpScript.Create(code, scriptOptions, typeof(Globals));
        var globals = new Globals { input = input };

        var result = script.RunAsync(globals).Result;
        return result.ReturnValue?.ToString();
    }
}