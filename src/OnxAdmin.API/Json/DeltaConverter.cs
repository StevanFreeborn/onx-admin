namespace OnxAdmin.API.Json;

class DeltaConverter : JsonConverter<ContentDelta>
{
  public override ContentDelta Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var jsonDocument = JsonDocument.ParseValue(ref reader);
    var root = jsonDocument.RootElement;
    var type = root.GetProperty("type").GetString();
    return type switch
    {
      ContentDeltaType.TextDelta => JsonSerializer.Deserialize<TextDelta>(root.GetRawText(), options)!,
      ContentDeltaType.JsonDelta => JsonSerializer.Deserialize<JsonDelta>(root.GetRawText(), options)!,
      _ => throw new JsonException($"Unknown content type: {type}")
    };
  }

  public override void Write(Utf8JsonWriter writer, ContentDelta value, JsonSerializerOptions options)
  {
    JsonSerializer.Serialize(writer, value, value.GetType(), options);
  }
}