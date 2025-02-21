using EtlApp.Domain.Dto;

namespace EtlApp.Domain.Target;

public interface ITargetConnection
{
    public void Upload(ReportData data);
}