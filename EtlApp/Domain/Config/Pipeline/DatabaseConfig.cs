using System.Text.Json.Serialization;
using EtlApp.Domain.Config.Pipeline.Converter;

namespace EtlApp.Domain.Config.Pipeline;

[JsonConverter(typeof(DatabaseConfigConverter))]
public class DatabaseConfig
{
    public required string Type { get; set; }
}