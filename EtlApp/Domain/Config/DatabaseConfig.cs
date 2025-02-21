using System.Text.Json.Serialization;

namespace EtlApp.Domain.Config;

[JsonConverter(typeof(SourceConfigConverter))]
public class DatabaseConfig
{
}