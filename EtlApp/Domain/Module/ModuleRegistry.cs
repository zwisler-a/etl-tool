using EtlApp.Domain.Database;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;

namespace EtlApp.Domain.Module;

public class ModuleRegistry
{
    public readonly TargetConnectionFactory TargetFactory = new();
    public readonly SourceConnectionFactory SourceFactory = new();
    public readonly DatabaseManager DatabaseManager = new();

    public void RegisterModule(Module module)
    {
        module.RegisterSourceConnection(SourceFactory);
        module.RegisterTargetConnection(TargetFactory);
        module.RegisterConnections(DatabaseManager);
    }
    
    
}