namespace EtlApp.Domain.Config;

public class PipelineConfiguration
{
    public required List<SourceConfig> Source { get; set; }
    public required List<TargetConfig> Target { get; set; }
    public required List<string> Middleware { get; set; }
}