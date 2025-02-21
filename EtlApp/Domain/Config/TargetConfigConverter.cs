using System.Text.Json;
using System.Text.Json.Serialization;
using EtlApp.Config;

public class TargetConfigConverter : JsonConverter<TargetConfig>
{
    // Static dictionary to hold type registrations
    private static readonly Dictionary<string, Func<JsonElement, TargetConfig>> TypeRegistry = new();

    static TargetConfigConverter()
    {
    }

    // Method to register child types
    public static void Register<T>(string type) where T : TargetConfig
    {
        TypeRegistry[type] = (json) => JsonSerializer.Deserialize<T>(json.GetRawText());
    }

    public override TargetConfig Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("Type", out var typeProp))
        {
            throw new JsonException("Missing 'Type' property.");
        }

        string type = typeProp.GetString()!;

        // Use the registry to find the correct type to deserialize into
        if (TypeRegistry.TryGetValue(type, out var creator))
        {
            return creator(root);
        }

        throw new JsonException($"Unknown Type '{type}'");
    }

    public override void Write(Utf8JsonWriter writer, TargetConfig value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}