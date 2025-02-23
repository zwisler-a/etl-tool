using EtlApp.Domain.Dto;

namespace EtlApp.Adapter.Sql;

public class SqlTypes
{
    public static readonly Dictionary<ColumnType, string> ColumnTypeToSqlType = new()
    {
        { ColumnType.String, "varchar(255)" },
        { ColumnType.Int, "int" },
        { ColumnType.Decimal, "DECIMAL(24, 6)" },
        { ColumnType.DateTime, "datetime" },
        { ColumnType.Date, "date" },
        { ColumnType.Boolean, "bit" }
    };

    public static readonly Dictionary<Type, ColumnType> DataTypeToSqlType = new()
    {
        { typeof(string), ColumnType.String },
        { typeof(decimal), ColumnType.Decimal },
        { typeof(float), ColumnType.Decimal },
        { typeof(double), ColumnType.Decimal },
        { typeof(int), ColumnType.Int },
        { typeof(long), ColumnType.Int },
        { typeof(short), ColumnType.Int },
        { typeof(DateTime), ColumnType.DateTime },
        { typeof(DateOnly), ColumnType.Date },
        { typeof(bool), ColumnType.Boolean }
    };
}