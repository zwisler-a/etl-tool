using EtlApp.Domain.Config.Pipeline.Converter;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Module;

namespace EtlApp.Tests.Adapter.TestAdapter;

public class TestModule : Module
{
    public override void RegisterSourceConnection(SourceConnectionFactory factory)
    {
        SourceConfigConverter.Register<TestSourceConfig>("test");
        factory.Register((TestSourceConfig c, PipelineContext context) => new TestSource(c, context));
    }

    public override void RegisterTargetConnection(TargetConnectionFactory factory)
    {
        TargetConfigConverter.Register<TestTargetConfig>("test");
        factory.Register((TestTargetConfig c, PipelineContext context) => new TestTarget(c, context));
    }

    public override void RegisterConnections(DatabaseManager databaseManager)
    {
    }

    public override void RegisterMiddleware(MiddlewareFactory middlewareFactory)
    {
    }

    public override void RegisterTransformer(TransformerFactory transformerFactory)
    {
    }
}