using System.Data;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Util.Observable;
using Manatee.Json.Transform;
using Newtonsoft.Json;
using JsonValue = Manatee.Json.JsonValue;

namespace EtlApp.Adapter.Rest;

public class RestSourceConnectionConfig : SourceConfig
{
    public JsonValue? Jslt;
    public required string BaseUrl { get; set; }
    public required string Endpoint { get; set; }
    public string Method { get; set; } = "GET";
    public Dictionary<string, string> Headers { get; set; } = new();
}

public class RestSourceConnection : Observable<ReportData>, ISourceConnection
{
    private readonly RestSourceConnectionConfig _config;
    private readonly PipelineContext _context;
    private readonly HttpClient _httpClient;

    public RestSourceConnection(RestSourceConnectionConfig config, PipelineContext context)
    {
        _config = config;
        _context = context;
        _httpClient = new HttpClient { BaseAddress = new Uri(_config.BaseUrl) };

        foreach (var header in _config.Headers)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    public void Fetch()
    {
        try
        {
            var request = new HttpRequestMessage(new HttpMethod(_config.Method), _config.Endpoint);
            var response = _httpClient.Send(request);
            response.EnsureSuccessStatusCode();

            var jsonData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();


            if (_config.Jslt != null)
            {
                jsonData = JsonTransformer.Transform(jsonData, _config.Jslt).ToString();
            }


            var transformedList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonData);
            transformedList.First().Keys.Select(key => new ColumnMappingConfig()
            {
                SourceName = key.ToString(),
                SourceType = ColumnType.Undefined
            });
            
            DataTable dataTable = ConvertToDataTable(transformedList);
            var mappings = new Dictionary<string, ColumnMappingConfig>();
            foreach (DataColumn dataTableColumn in dataTable.Columns)
            {
                mappings[dataTableColumn.ColumnName] = _context.GetColumnMapping(dataTableColumn.ColumnName);
            }

            Next(new ReportData(dataTable, mappings));
        }
        catch (Exception ex)
        {
            Error(ex);
            throw;
        }
    }

    private DataTable ConvertToDataTable(List<Dictionary<string, object>> list)
    {
        DataTable table = new DataTable();

        if (list == null || list.Count == 0)
            return table;

        // Create columns dynamically
        foreach (var column in list[0].Keys)
        {
            table.Columns.Add(column, typeof(object));
        }

        // Add rows
        foreach (var dict in list)
        {
            var row = table.NewRow();
            foreach (var kvp in dict)
            {
                row[kvp.Key] = kvp.Value ?? DBNull.Value;
            }

            table.Rows.Add(row);
        }

        return table;
    }
}