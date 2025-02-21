using EtlApp.Adapter.Sql;
using EtlApp.Domain.Dto;

namespace EtlApp.Domain.Target;

public class SqlTargetConnection(SqlTargetConfig config): ITargetConnection
{
    public void Upload(ReportData data)
    {
        throw new NotImplementedException();
    }
}