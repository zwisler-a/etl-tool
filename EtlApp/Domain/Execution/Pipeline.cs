using EtlApp.Domain.Dto;
using EtlApp.Domain.Middleware;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Execution;

public class Pipeline
{
    private readonly ISourceConnection _source;
    private readonly ITargetConnection _target;
    private List<IMiddleware> _middlewares;
    private readonly PipelineExecutionContext _context;

    private readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger<Pipeline>();

    public Pipeline(ISourceConnection source, ITargetConnection target, PipelineExecutionContext context,
        List<IMiddleware> middlewares)
    {
        _source = source;
        _target = target;
        _context = context;
        _middlewares = middlewares;
    }

    public List<ExecutionIssue> Run()
    {
        var executionIssues = new List<ExecutionIssue>();
        _logger.LogInformation("Starting execution ...");
        try
        {
            _logger.LogInformation("Fetching data ...");
            var reportData = _source.Fetch(_context);
            _logger.LogInformation("Found {} report(s)", reportData.Count);
            foreach (var report in reportData)
            {
                var processed = report;
                foreach (var mw in _middlewares)
                {
                    _logger.LogInformation("Applied middleware ...");
                    processed = mw.Process(processed, _context);
                }

                _logger.LogInformation("Uploading report ...");
                _target.Upload(report, _context);
                _logger.LogInformation("Done");
            }
        }
        catch (Exception ex)
        {
            executionIssues.Add(new ExecutionIssue(ex.Message));
            _logger.LogError("An error occured: {}", ex);
            return executionIssues;
        }


        return executionIssues;
    }
}