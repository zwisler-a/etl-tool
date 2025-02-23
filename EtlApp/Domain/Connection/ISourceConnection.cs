using EtlApp.Domain.Dto;

namespace EtlApp.Domain.Connection;

public interface ISourceConnection: IObservable<ReportData>
{
    public void Fetch();
}