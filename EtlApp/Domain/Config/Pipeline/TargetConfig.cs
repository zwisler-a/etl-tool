using System.Text.Json.Serialization;
using EtlApp.Domain.Config.Pipeline.Converter;
using EtlApp.Domain.Dto;

namespace EtlApp.Domain.Config.Pipeline;

[JsonConverter(typeof(TargetConfigConverter))]
public class TargetConfig
{
    public required UpdateStrategy UpdateStrategy { get; set; }
    public required string Type { get; set; }
}