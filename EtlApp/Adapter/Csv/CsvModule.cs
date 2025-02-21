using System.Data.Common;
using EtlApp.Domain.Config;
using EtlApp.Domain.Module;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;

namespace EtlApp.Adapter.Csv;

public class CsvModule: Module
{
    
    public override Func<SourceConfig, ISourceConnection>? SourceConnection { get; }
    public override Func<TargetConfig, ITargetConnection>? TargetConnection { get; }
    public override Dictionary<string, DbConnection> Connections { get; }
    
    public CsvModule()
    {
        SourceConnection = (CsvSourceConfig config) => new CsvSourceConnection(config);
    }


}