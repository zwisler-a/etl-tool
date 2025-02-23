using EtlApp.Domain.Config.Pipeline;

namespace EtlApp.Adapter.Csv;

public class CsvSourceConfig : SourceConfig
{
    public required string FilePath { get; set; }
    public required string FilePrefix { get; set; }
    public required string Delimiter { get; set; }
    public int BatchSize { get; set; } = 0;
    public bool MoveToArchive { get; set; } = false;
    public string? FailedArchivePath { get; set; }
    public string? SuccessArchivePath { get; set; }
}