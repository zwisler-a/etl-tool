using EtlApp.Domain.Dto;

namespace EtlApp.Domain.Source;

public interface ISourceConnection
{
    public List<ReportData> Fetch();
}