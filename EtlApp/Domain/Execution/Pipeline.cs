using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Middleware;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Execution;

public class Pipeline
{
    private readonly List<ISourceConnection> _sources;
    private readonly List<ITargetConnection> _targets;
    private readonly List<IMiddleware> _middlewares;
    private readonly PipelineExecutionContext _context;

    private readonly ILogger _logger;

    public Pipeline(PipelineExecutionContext context,
        List<IMiddleware> middlewares, List<ISourceConnection> sources, List<ITargetConnection> targets)
    {
        _context = context;
        _middlewares = middlewares;
        _sources = sources;
        _targets = targets;
        _logger = Logging.LoggerFactory.CreateLogger(ToString());;
    }

    public List<ExecutionIssue> Run()
    {
        var executionIssues = new List<ExecutionIssue>();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogDebug("Execution started.");

        try
        {
            _logger.LogDebug("Fetching data ...");
            var reportData = _sources.SelectMany(source => source.Fetch(_context)).ToList();
            _logger.LogDebug("Found {Count} report(s)", reportData.Count);

            foreach (var report in reportData)
            {
                var processedReport = report;
                foreach (var middleware in _middlewares)
                {
                    _logger.LogDebug("Applying middleware {MiddlewareName}...", middleware.GetType().Name);
                    processedReport = middleware.Process(processedReport, _context);
                }

                _logger.LogDebug("Uploading report ...");
                foreach (var target in _targets)
                {
                    _logger.LogDebug("Uploading to target {TargetName}...", target.GetType().Name);
                    target.Upload(processedReport, _context);
                }

                _logger.LogDebug("Report processing completed.");
            }
        }
        catch (Exception ex)
        {
            executionIssues.Add(new ExecutionIssue(ex.Message));
            _logger.LogError("An error occurred: {Error}", ex);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("Execution finished in {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        }

        return executionIssues;
    }

    public sealed override string ToString()
    {
        return
            $"[{string.Join(",", _sources.Select(s => s.GetType().Name))}]->[{string.Join(",", _targets.Select(s => s.GetType().Name))}]";
    }
}