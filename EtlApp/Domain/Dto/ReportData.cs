using System.Data;
using EtlApp.Domain.Config.Pipeline;

namespace EtlApp.Domain.Dto;

public record ReportData(
    DataTable Data,
    Dictionary<string, ColumnMappingConfig> Columns
);