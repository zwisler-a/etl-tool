using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtlApp.Domain.Config;

public static class ConfigurationManager
{
    private static readonly Lazy<ApplicationConfiguration> Configuration = new(LoadConfiguration);

    public static string ConfigFilePath { get; set; } = "config.json";

    public static ApplicationConfiguration Config => Configuration.Value;

    private static ApplicationConfiguration LoadConfiguration()
    {
        try
        {
            var json = File.ReadAllText(ConfigFilePath);
            return JsonSerializer.Deserialize<ApplicationConfiguration>(json, new JsonSerializerOptions
            {
                Converters =
                {
                    new SourceConfigConverter(), new TargetConfigConverter(), new DatabaseConfigConverter(),
                    new JsonStringEnumConverter()
                },
                PropertyNameCaseInsensitive = true
            }) ?? throw new NullReferenceException();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
            throw;
        }
    }

    public static PipelineConfiguration LoadPipelineConfiguration(string file)
    {
        try
        {
            var json = File.ReadAllText(file);
            return JsonSerializer.Deserialize<PipelineConfiguration>(json, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new SourceConfigConverter(), new TargetConfigConverter(), new DatabaseConfigConverter(),
                    new JsonStringEnumConverter()
                },
                PropertyNameCaseInsensitive = true
            }) ?? throw new NullReferenceException();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configuration: {ex.Message}");
            throw;
        }
    }
}