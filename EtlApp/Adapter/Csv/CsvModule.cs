using System.Data.Common;
using EtlApp.Domain.Config;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Database;
using EtlApp.Domain.Dto;
using EtlApp.Domain.Module;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;

namespace EtlApp.Adapter.Csv;

public class CsvModule : Module
{
    public override void RegisterSourceConnection(SourceConnectionFactory factory)
    {
        SourceConfigConverter.Register<CsvSourceConfig>("csv");
        factory.Register((CsvSourceConfig config, PipelineContext context) => new CsvSourceConnection(config, context));
    }

    public override void RegisterTargetConnection(TargetConnectionFactory factory)
    {
        TargetConfigConverter.Register<CsvTargetConfig>("csv");
        factory.Register((CsvTargetConfig config, PipelineContext context) => new CsvTargetConnection(config));
    }
}