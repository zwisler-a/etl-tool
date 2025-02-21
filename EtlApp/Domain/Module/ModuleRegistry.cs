using System.Diagnostics;
using EtlApp.Domain.Database;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;

namespace EtlApp.Domain.Module;

public class ModuleRegistry
{
    public bool RegisterModule(Module module)
    {
        Debug.Assert(module.SourceConnection<A>() != null, "module.SourceConnection != null");
        foreach (var factory in module.SourceConnection)
        {
            SourceConnectionFactory.Register(factory);
        }

        Debug.Assert(module.TargetConnection != null, "module.SourceConnection != null");
        foreach (var factory in module.TargetConnection)
        {
            TargetConnectionFactory.Register(factory);
        }

        foreach (var (key, connection) in module.Connections)
        {
            DatabaseManager.RegisterConnection(key, connection);
        }

        return true;
    }
}