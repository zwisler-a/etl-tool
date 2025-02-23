using System.Text.Json.Serialization;
using EtlApp.Domain.Config.Pipeline.Converter;

namespace EtlApp.Domain.Config.Pipeline;

[JsonConverter(typeof(SourceConfigConverter))]
public class SourceConfig
{
    public required string Type { get; set; }
}