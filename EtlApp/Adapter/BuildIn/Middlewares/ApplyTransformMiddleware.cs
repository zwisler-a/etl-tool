using System.Data;
using System.Diagnostics;
using EtlApp.Domain.Config;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Dto;
using Microsoft.Extensions.Logging;

namespace EtlApp.Adapter.BuildIn.Middlewares;

public class ApplyTransformMiddlewareConfig : MiddlewareConfig;

public class ApplyTransformMiddleware(ApplyTransformMiddlewareConfig config, PipelineContext context) : Domain.Connection.Middleware
{
    private readonly ILogger _logger = Logging.LoggerFactory.CreateLogger<DetectTypeMiddleware>();

    protected override ReportData Process(ReportData reportData)
    {
        if (context.Transformers.Keys.Count == 0)
        {
            return reportData;
        }

        foreach (DataColumn dataColumn in reportData.Data.Columns)
        {
            dataColumn.ReadOnly = false;
        }

        foreach (DataRow originalRow in reportData.Data.Rows)
        {
            foreach (var (columnName, columnConfig) in reportData.Columns)
            {
                if (columnConfig.Transform is { Count: > 0 })
                {
                    originalRow[columnName] = ConvertValue(originalRow[columnName], columnConfig, context);
                }
            }
        }

        foreach (DataColumn dataColumn in reportData.Data.Columns)
        {
            dataColumn.ReadOnly = true;
        }

        return reportData;
    }

    private object? ConvertValue(object? value, ColumnMappingConfig config, PipelineContext context)
    {
        if (config.Transform != null)
        {
            var transformers = config.Transform.Select(s => context.Transformers.GetValueOrDefault(s));
            var transVal = value;
            foreach (var transformer in transformers)
            {
                Debug.Assert(transformer != null, nameof(transformer) + "Missing Transformer");
                transVal = transformer.Transform(transVal);
            }

            return transVal;
        }

        return value;
    }
}