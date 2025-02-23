using EtlApp.Domain.Dto;
using EtlApp.Domain.Execution;

namespace EtlApp.Domain.Connection;

public interface ISourceConnection: IObservable<ReportData>
{
    public void Fetch();
}