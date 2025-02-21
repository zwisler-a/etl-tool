using System.Data;

namespace EtlApp.Domain.Dto;

public record ReportData(
    DataTable Data,
    Dictionary<string, ColumnType> Columns
);