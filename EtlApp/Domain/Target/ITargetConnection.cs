using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;

namespace EtlApp.Domain.Target;

public interface ITargetConnection
{
    public void Upload(ReportData data, PipelineExecutionContext context);
}