
namespace OnxAdmin.API.Json;

class ContentConverter : JsonConverter<Content>
{
  public override Content Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var jsonDocument = JsonDocument.ParseValue(ref reader);
    var rootElement = jsonDocument.RootElement;
    var type = rootElement.GetProperty("type").GetString()!;

    return type switch
    {
      ContentType.Text => JsonSerializer.Deserialize<TextContent>(rootElement.GetRawText(), options)!,
      ContentType.Image => JsonSerializer.Deserialize<ImageContent>(rootElement.GetRawText(), options)!,
      ContentType.ToolUse => JsonSerializer.Deserialize<ToolUseContent>(rootElement.GetRawText(), options)!,
      ContentType.ToolResult => JsonSerializer.Deserialize<ToolResultContent>(rootElement.GetRawText(), options)!,
      _ => throw new JsonException()
    };
  }

  public override void Write(Utf8JsonWriter writer, Content value, JsonSerializerOptions options)
  {
    JsonSerializer.Serialize(writer, value, value.GetType(), options);
  }
}