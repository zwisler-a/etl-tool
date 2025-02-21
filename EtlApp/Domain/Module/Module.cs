using System.Data.Common;
using EtlApp.Domain.Config;
using EtlApp.Domain.Source;
using EtlApp.Domain.Target;

namespace EtlApp.Domain.Module;

public abstract class Module
{
    public abstract Func<SourceConfig, ISourceConnection>? SourceConnection { get; }
    public abstract Func<TargetConfig, ITargetConnection>? TargetConnection { get; }
    public abstract Dictionary<string, DbConnection> Connections { get; }
}