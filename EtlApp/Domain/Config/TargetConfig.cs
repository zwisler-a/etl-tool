using System.Text.Json.Serialization;

namespace EtlApp.Config;

[JsonConverter(typeof(SourceConfigConverter))]
public class TargetConfig
{
    public required string Type { get; set; }
}