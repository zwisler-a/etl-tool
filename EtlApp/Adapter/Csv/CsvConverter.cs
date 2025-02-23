using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace EtlApp.Adapter.Csv;

using CsvHelper.TypeConversion;

public class CustomDateTimeConverter : ITypeConverter
{
    private readonly string _format;
    
    public CustomDateTimeConverter(string format)
    {
        _format = format;
    }

    public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData) 
        => DateTime.ParseExact(text, _format, CultureInfo.InvariantCulture);

    public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        => value is DateTime dt ? dt.ToString(_format, CultureInfo.InvariantCulture) : string.Empty;
}

public class CustomDateOnlyConverter : ITypeConverter
{
    private readonly string _format;

    public CustomDateOnlyConverter(string format)
    {
        _format = format;
    }

    public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData) 
        => DateOnly.ParseExact(text, _format, CultureInfo.InvariantCulture);

    public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        => value is DateOnly d ? d.ToString(_format, CultureInfo.InvariantCulture) : string.Empty;
}