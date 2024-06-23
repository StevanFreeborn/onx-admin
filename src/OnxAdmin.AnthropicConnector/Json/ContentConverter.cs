using System.Text.Json;
using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Models;

namespace OnxAdmin.AnthropicConnector.Json;

class ContentConverter : JsonConverter<Content>
{
  public override Content Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var jsonDocument = JsonDocument.ParseValue(ref reader);
    var root = jsonDocument.RootElement;
    var type = root.GetProperty("type").GetString();
    return type switch
    {
      ContentType.Text => JsonSerializer.Deserialize<TextContent>(root.GetRawText(), options)!,
      ContentType.Image => JsonSerializer.Deserialize<ImageContent>(root.GetRawText(), options)!,
      ContentType.ToolUse => JsonSerializer.Deserialize<ToolUseContent>(root.GetRawText(), options)!,
      ContentType.ToolResult => JsonSerializer.Deserialize<ToolResultContent>(root.GetRawText(), options)!,
      _ => throw new JsonException($"Unknown content type: {type}")
    };
  }

  public override void Write(Utf8JsonWriter writer, Content value, JsonSerializerOptions options)
  {
    JsonSerializer.Serialize(writer, value, value.GetType(), options);
  }
}