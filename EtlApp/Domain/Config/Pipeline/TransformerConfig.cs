using System.Text.Json.Serialization;
using EtlApp.Domain.Config.Pipeline.Converter;

namespace EtlApp.Domain.Config.Pipeline;

[JsonConverter(typeof(TransformerConfigConverter))]
public class TransformerConfig
{
    public required string Type { get; set; }
    public required string Name { get; set; }
}