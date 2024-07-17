namespace OnxAdmin.API.Json;

class EventDataConverter : JsonConverter<EventData>
{
  public override EventData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var jsonDocument = JsonDocument.ParseValue(ref reader);
    var root = jsonDocument.RootElement;
    var type = root.GetProperty("type").GetString();
    return type switch
    {
      EventType.Ping => JsonSerializer.Deserialize<PingEventData>(root.GetRawText(), options)!,
      EventType.Error => JsonSerializer.Deserialize<ErrorEventData>(root.GetRawText(), options)!,
      EventType.MessageStart => JsonSerializer.Deserialize<MessageStartEventData>(root.GetRawText(), options)!,
      EventType.MessageDelta => JsonSerializer.Deserialize<MessageDeltaEventData>(root.GetRawText(), options)!,
      EventType.MessageStop => JsonSerializer.Deserialize<MessageStopEventData>(root.GetRawText(), options)!,
      EventType.ContentBlockStart => JsonSerializer.Deserialize<ContentStartEventData>(root.GetRawText(), options)!,
      EventType.ContentBlockDelta => JsonSerializer.Deserialize<ContentDeltaEventData>(root.GetRawText(), options)!,
      EventType.ContentBlockStop => JsonSerializer.Deserialize<ContentStopEventData>(root.GetRawText(), options)!,
      EventType.MessageComplete => JsonSerializer.Deserialize<MessageCompleteEventData>(root.GetRawText(), options)!,
      _ => throw new JsonException($"Unknown content type: {type}")
    };
  }

  public override void Write(Utf8JsonWriter writer, EventData value, JsonSerializerOptions options)
  {
    JsonSerializer.Serialize(writer, value, value.GetType(), options);
  }
}