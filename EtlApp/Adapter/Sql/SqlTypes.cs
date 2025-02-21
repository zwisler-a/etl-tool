using EtlApp.Domain.Dto;

namespace EtlApp.Adapter.Sql;

public class SqlTypes
{
    public static readonly Dictionary<ColumnType, string> DataTypes = new()
    {
        { ColumnType.String, "varchar(255)" },
        { ColumnType.Int, "int" },
        { ColumnType.DateTime, "datetime" },
    };
}