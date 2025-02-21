using EtlApp.Domain.Dto;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;

namespace EtlApp.Domain.Execution;

public class Pipeline
{
    private ISourceConnection _source;
    private ITargetConnection _target;

    public Pipeline(ISourceConnection source, ITargetConnection target)
    {
        _source = source;
        _target = target;
    }

    public List<ExecutionIssue> Run()
    {
        var executionIssues = new List<ExecutionIssue>();

        try
        {
            var reportData = _source.Fetch();
            foreach (var report in reportData)
            {
                _target.Upload(report);
            }
        }
        catch (Exception ex)
        {
            executionIssues.Add(new ExecutionIssue(ex.Message));
            return executionIssues;
        }


        return executionIssues;
    }
}