using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Dto;
using EtlApp.Util;

namespace EtlApp.Domain.Connection;

public class TargetConnectionFactory : Factory<TargetConfig, PipelineContext, ITargetConnection>;