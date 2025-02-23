namespace EtlApp.Util;

using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonDerivedTypeConverter<TBase> : JsonConverter<TBase> where TBase : class
{
    private static readonly Dictionary<string, Func<JsonElement, TBase>> TypeRegistry = new();

    static JsonDerivedTypeConverter()
    {
    }

    public static void Register<T>(string type) where T : TBase
    {
        TypeRegistry[type] = (json) => JsonSerializer.Deserialize<T>(json.GetRawText(),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                },
            })!;
    }

    public override TBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("Type", out var typeProp))
        {
            throw new JsonException("Missing 'Type' property.");
        }

        string type = typeProp.GetString()!;

        if (TypeRegistry.TryGetValue(type, out var creator))
        {
            return creator(root);
        }

        throw new JsonException($"Unknown Type '{type}'");
    }

    public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}