using EtlApp.Domain.Config;

namespace EtlApp.Adapter.Csv;

public class CsvTargetConfig : TargetConfig
{
    public required string FilePath { get; set; }
    public required string FilePrefix { get; set; }
    public required string Delimiter { get; set; }
}