using EtlApp.Domain.Dto;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Execution;

public class Pipeline
{
    private ISourceConnection _source;
    private ITargetConnection _target;
    private readonly PipelineExecutionContext _context;

    private readonly ILogger _logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger<Pipeline>();

    public Pipeline(ISourceConnection source, ITargetConnection target, PipelineExecutionContext context)
    {
        _source = source;
        _target = target;
        _context = context;
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