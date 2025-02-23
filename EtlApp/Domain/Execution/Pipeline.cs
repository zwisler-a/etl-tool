using System.Diagnostics;
using EtlApp.Domain.Config;
using EtlApp.Domain.Connection;
using EtlApp.Util.Observable;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Execution;

public class Pipeline
{
    
    private static int _counter = 0;
    private int _id;
    
    private readonly List<ISourceConnection> _sources;
    private readonly List<ITargetConnection> _targets;
    private readonly List<IMiddleware> _middlewares;
    

    private readonly ILogger _logger;

    public Pipeline(
        List<IMiddleware> middlewares, List<ISourceConnection> sources, List<ITargetConnection> targets)
    {
        _middlewares = middlewares;
        _sources = sources;
        _targets = targets;
        _id = Interlocked.Increment(ref _counter);
        _logger = Logging.LoggerFactory.CreateLogger($"{ToString()}[{_id}]");
    }

    public void Run()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Start execution");
        var counter = new RowCounterObserver(_logger);
        try
        {
            foreach (var sourceConnection in _sources)
            {
                var pipes = ObservableUtils.Concat([.._middlewares]);
                sourceConnection.Subscribe(pipes.Observer);


                foreach (var targetConnection in _targets)
                {
                    pipes.Observable.Subscribe(targetConnection);
                }

                _logger.LogDebug("Fetching data ...");
                pipes.Observable.Subscribe(counter);

                sourceConnection.Fetch();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred.");
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("Execution finished: {} chunks with {} rows to {} target(s) in {} ms",
                counter.ReportCount,
                counter.RowCount, _targets.Count,
                stopwatch.ElapsedMilliseconds);
        }
    }

    public sealed override string ToString()
    {
        return
            $"[{string.Join(",", _sources.Select(s => s.GetType().Name))}]->[{string.Join(",", _targets.Select(s => s.GetType().Name))}]";
    }
}