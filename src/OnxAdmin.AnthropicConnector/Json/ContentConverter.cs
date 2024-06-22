using System.Text.Json;
using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Models;

namespace OnxAdmin.AnthropicConnector.Json;

public class ContentConverter : JsonConverter<Content>
{
  public override Content Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var jsonDocument = JsonDocument.ParseValue(ref reader);
    var root = jsonDocument.RootElement;

    var type = root.GetProperty("type").GetString();
    return type switch
    {
      "text" => JsonSerializer.Deserialize<TextContent>(root.GetRawText(), options)!,
      "image" => JsonSerializer.Deserialize<ImageContent>(root.GetRawText(), options)!,
      _ => throw new JsonException($"Unknown content type: {type}")
    };
  }

  public override void Write(Utf8JsonWriter writer, Content value, JsonSerializerOptions options)
  {
    JsonSerializer.Serialize(writer, value, value.GetType(), options);
  }
}