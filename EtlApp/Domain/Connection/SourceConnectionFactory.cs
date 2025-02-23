using EtlApp.Domain.Config;
using EtlApp.Domain.Dto;
using EtlApp.Util;

namespace EtlApp.Domain.Connection
{
    public class SourceConnectionFactory : Factory<SourceConfig, PipelineContext, ISourceConnection>;
}