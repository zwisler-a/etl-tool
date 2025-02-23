using EtlApp.Adapter.BuildIn.Middlewares;
using EtlApp.Adapter.BuildIn.Transformation;
using EtlApp.Domain.Config;
using EtlApp.Domain.Config.Pipeline.Converter;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Module;

namespace EtlApp.Adapter.BuildIn;

public class BuildInModule : Module
{
    public override void RegisterTransformer(TransformerFactory transformerFactory)
    {
        TransformerConfigConverter.Register<RegexReplaceTransformerConfig>("regex_replace");
        transformerFactory.Register((RegexReplaceTransformerConfig config, PipelineContext _) =>
            new RegexReplaceTransformer(config));
    }

    public override void RegisterMiddleware(MiddlewareFactory middlewareFactory)
    {
        MiddlewareConfigConverter.Register<DetectTypeMiddlewareConfig>("type_inference");
        middlewareFactory
            .Register((DetectTypeMiddlewareConfig c, PipelineContext co) => new DetectTypeMiddleware(c, co));

        MiddlewareConfigConverter.Register<CastTypeMiddlewareConfig>("type_cast");
        middlewareFactory
            .Register((CastTypeMiddlewareConfig c, PipelineContext co) => new CastTypeMiddleware(c, co));

        MiddlewareConfigConverter.Register<ValidateTypeMiddlewareConfig>("type_validation");
        middlewareFactory
            .Register((ValidateTypeMiddlewareConfig c, PipelineContext co) => new ValidateTypeMiddleware(c, co));

        MiddlewareConfigConverter.Register<ApplyTransformMiddlewareConfig>("apply_transformers");
        middlewareFactory
            .Register((ApplyTransformMiddlewareConfig c, PipelineContext co) => new ApplyTransformMiddleware(c, co));
    }
}