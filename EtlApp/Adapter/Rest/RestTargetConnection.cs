using System.Data;
using System.Text;
using EtlApp.Domain.Config.Pipeline;
using EtlApp.Domain.Connection;
using EtlApp.Domain.Dto;
using EtlApp.Util.Observable;
using Manatee.Json.Transform;
using Newtonsoft.Json;
using JsonValue = Manatee.Json.JsonValue;

namespace EtlApp.Adapter.Rest;

public class RestTargetConnectionConfig : TargetConfig
{
    public required string BaseUrl { get; set; }
    public required string Endpoint { get; set; }
    public string Method { get; set; } = "PUT";
    public Dictionary<string, string> Headers { get; set; } = new();
}

public class RestTargetConnection : ITargetConnection
{
    private readonly RestTargetConnectionConfig _config;
    private readonly PipelineContext _context;
    private readonly HttpClient _httpClient;

    public RestTargetConnection(RestTargetConnectionConfig config, PipelineContext context)
    {
        _config = config;
        _context = context;
        _httpClient = new HttpClient { BaseAddress = new Uri(_config.BaseUrl) };

        foreach (var header in _config.Headers)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }
    
    public void OnNext(ReportData value)
    {
        var request = new HttpRequestMessage(new HttpMethod(_config.Method), _config.Endpoint);
        
        if (_config.Method != "GET")
        {
            var jsonData = JsonConvert.SerializeObject(value);
            request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        }
        
        var response = _httpClient.Send(request);
        response.EnsureSuccessStatusCode();
    }
    
    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public List<UpdateStrategy> GetSupportedUpdateStrategies()
    {
        return [UpdateStrategy.ReplaceComplete];
    }
}