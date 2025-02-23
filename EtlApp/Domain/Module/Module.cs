using EtlApp.Domain.Connection;
using EtlApp.Domain.Database;

namespace EtlApp.Domain.Module;

public abstract class Module
{
    public virtual void RegisterSourceConnection(SourceConnectionFactory factory)
    {
    }

    public virtual void RegisterTargetConnection(TargetConnectionFactory factory)
    {
    }

    public virtual void RegisterConnections(DatabaseManager databaseManager)
    {
    }

    public virtual void RegisterMiddleware(MiddlewareFactory middlewareFactory)
    {
    }
    
    public virtual void RegisterTransformer(TransformerFactory transformerFactory)
    {
    }
}