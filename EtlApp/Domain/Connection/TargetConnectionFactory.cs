using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Target;
using EtlApp.Util;

namespace EtlApp.Domain.Connection;

public class TargetConnectionFactory : Factory<TargetConfig, PipelineContext, ITargetConnection>;