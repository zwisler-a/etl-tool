using System.Text.Json.Serialization;

namespace EtlApp.Domain.Config;

[JsonConverter(typeof(DatabaseConfigConverter))]
public class DatabaseConfig
{
    public required string Type { get; set; }
}