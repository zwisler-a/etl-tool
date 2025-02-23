using EtlApp.Domain.Dto;

namespace EtlApp.Domain.Target;

public interface ITargetConnection : IObserver<ReportData>
{
    public List<UpdateStrategy> GetSupportedUpdateStrategies();

}