using EtlApp.Domain.Config.Pipeline.Converter;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Module;

namespace EtlApp.Adapter.Rest;

public class RestModule : Module
{
    public override void RegisterSourceConnection(SourceConnectionFactory factory)
    {
        SourceConfigConverter.Register<RestSourceConnectionConfig>("rest");
        factory.Register((RestSourceConnectionConfig config, PipelineContext context) =>
            new RestSourceConnection(config, context));
    }
}