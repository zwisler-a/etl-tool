using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Dto;
using EtlApp.Util;

namespace EtlApp.Domain.Connection;

public class MiddlewareFactory : Factory<MiddlewareConfig, PipelineContext, IMiddleware>;