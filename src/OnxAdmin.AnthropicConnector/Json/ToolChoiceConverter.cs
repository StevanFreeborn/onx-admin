using System.Text.Json;
using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Models;

namespace OnxAdmin.AnthropicConnector.Json;

class ToolChoiceConverter : JsonConverter<ToolChoice>
{
  public override ToolChoice Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var jsonDocument = JsonDocument.ParseValue(ref reader);
    var root = jsonDocument.RootElement;

    var type = root.GetProperty("type").GetString();

    return type switch
    {
      ToolChoiceType.Auto => JsonSerializer.Deserialize<AutoToolChoice>(root.GetRawText(), options)!,
      ToolChoiceType.Any => JsonSerializer.Deserialize<AnyToolChoice>(root.GetRawText(), options)!,
      ToolChoiceType.Tool => JsonSerializer.Deserialize<SpecificToolChoice>(root.GetRawText(), options)!,
      _ => throw new JsonException($"Unknown tool choice type: {type}")
    };
  }

  public override void Write(Utf8JsonWriter writer, ToolChoice value, JsonSerializerOptions options)
  {
    JsonSerializer.Serialize(writer, value, value.GetType(), options);
  }
}