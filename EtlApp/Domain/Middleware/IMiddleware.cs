using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;

namespace EtlApp.Domain.Middleware;

public interface IMiddleware
{
    public ReportData Process(ReportData reportData, PipelineExecutionContext context);
}