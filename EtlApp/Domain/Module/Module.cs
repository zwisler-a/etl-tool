using EtlApp.Domain.Database;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;

namespace EtlApp.Domain.Module;

public interface Module
{
    public void RegisterSourceConnection(SourceConnectionFactory factory);
    public void RegisterTargetConnection(TargetConnectionFactory factory);
    public void RegisterConnections(DatabaseManager databaseManager);
}