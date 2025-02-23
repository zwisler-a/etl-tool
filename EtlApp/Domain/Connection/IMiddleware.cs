using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using EtlApp.Util.Observable;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Connection;

public interface IMiddleware : IPipeable<ReportData, ReportData>;

public abstract class Middleware : Observable<ReportData>, IMiddleware
{
    private readonly ILogger _logger = Logging.LoggerFactory.CreateLogger<Middleware>();
    protected abstract ReportData Process(ReportData reportData);

    public void OnCompleted()
    {
        Complete();
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(ReportData reportData)
    {
        _logger.LogDebug($"[Middleware - {GetType().Name}] Processing ...");
        Next(Process(reportData));
    }
}