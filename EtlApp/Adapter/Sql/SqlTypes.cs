using EtlApp.Domain.Dto;

namespace EtlApp.Adapter.Sql;

public class SqlTypes
{
    public static readonly Dictionary<ColumnType, string> DataTypes = new()
    {
        { ColumnType.String, "varchar(255)" },
        { ColumnType.Int, "int" },
        { ColumnType.Decimal, "DECIMAL(8, 3)" },
        { ColumnType.DateTime, "datetime" },
        { ColumnType.Date, "date" }
    };

    public static readonly Dictionary<Type, ColumnType> TypeDataTypes = new()
    {
        { typeof(string), ColumnType.String },
        { typeof(decimal), ColumnType.Decimal },
        { typeof(float), ColumnType.Decimal },
        { typeof(double), ColumnType.Decimal },
        { typeof(int), ColumnType.Int },
        { typeof(long), ColumnType.Int },
        { typeof(short), ColumnType.Int },
        { typeof(DateTime), ColumnType.DateTime },
        { typeof(DateOnly), ColumnType.Date }
    };
}