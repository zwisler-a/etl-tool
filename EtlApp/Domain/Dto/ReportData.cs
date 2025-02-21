using System.Data;

namespace EtlApp.Domain.Dto;

public record ReportData(
    DataTable Data,
    List<string>? ColumnNames
)
{
};