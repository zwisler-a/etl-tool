using System.Text.Json.Serialization;
using EtlApp.Domain.Config.Pipeline.Converter;

namespace EtlApp.Domain.Config.Pipeline;

[JsonConverter(typeof(MiddlewareConfigConverter))]
public class MiddlewareConfig
{
    public required string Type { get; set; }
}