using EtlApp.Domain.Config.Pipeline;
using Microsoft.Extensions.Logging;

namespace EtlApp.Domain.Config;

public class ApplicationConfiguration
{
    public required List<DatabaseConfig> Database { get; set; }
    public required string ConfigFolder { get; set; }
    public List<TransformerConfig>? Transformer { get; set; }
}