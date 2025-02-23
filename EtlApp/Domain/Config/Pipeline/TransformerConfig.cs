using System.Text.Json.Serialization;

namespace EtlApp.Domain.Config;

[JsonConverter(typeof(TransformerConfigConverter))]
public class TransformerConfig
{
    public required string Type { get; set; }
    public required string Name { get; set; }
}