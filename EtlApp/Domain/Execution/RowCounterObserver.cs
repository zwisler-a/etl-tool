using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Execution;

public class RowCounterObserver(ILogger logger) : IObserver<ReportData>
{
    public int RowCount;
    public int ReportCount;

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(ReportData reportData)
    {
        ReportCount++;
        RowCount += reportData.Data.Rows.Count;
        logger.LogInformation("Processed chunk {} with {}, total {}",
            ReportCount, reportData.Data.Rows.Count, RowCount);
    }
}