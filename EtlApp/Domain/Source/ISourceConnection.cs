using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;

namespace EtlApp.Domain.Source;

public interface ISourceConnection
{
    public List<ReportData> Fetch(PipelineExecutionContext context);
}