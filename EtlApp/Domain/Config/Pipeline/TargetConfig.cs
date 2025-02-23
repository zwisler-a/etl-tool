using System.Text.Json.Serialization;
using EtlApp.Domain.Dto;

namespace EtlApp.Domain.Config;

[JsonConverter(typeof(TargetConfigConverter))]
public class TargetConfig
{
    public required UpdateStrategy UpdateStrategy { get; set; }
    public required string Type { get; set; }
}