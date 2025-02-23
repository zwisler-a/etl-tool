using EtlApp.Domain.Dto;

namespace EtlApp.Domain.Connection;

public interface ITargetConnection : IObserver<ReportData>
{
    public List<UpdateStrategy> GetSupportedUpdateStrategies();

}