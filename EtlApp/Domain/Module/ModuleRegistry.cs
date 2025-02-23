using EtlApp.Domain.Connection;

namespace EtlApp.Domain.Module;

public class ModuleRegistry
{
    public readonly TargetConnectionFactory TargetFactory = new();
    public readonly SourceConnectionFactory SourceFactory = new();
    public readonly TransformerFactory TransformerFactory = new();
    public readonly DatabaseManager DatabaseManager = new();
    public readonly MiddlewareFactory MiddlewareFactory = new();

    public void RegisterModule(Module module)
    {
        module.RegisterSourceConnection(SourceFactory);
        module.RegisterTargetConnection(TargetFactory);
        module.RegisterConnections(DatabaseManager);
        module.RegisterMiddleware(MiddlewareFactory);
        module.RegisterTransformer(TransformerFactory);
    }
}