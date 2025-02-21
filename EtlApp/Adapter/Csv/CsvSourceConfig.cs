using EtlApp.Config;

namespace EtlApp.Domain.Source;

public class CsvSourceConfig : SourceConfig
{
    public required string FilePath { get; set; }
    public required string FilePrefix { get; set; }
    public required string Delimiter { get; set; }
}