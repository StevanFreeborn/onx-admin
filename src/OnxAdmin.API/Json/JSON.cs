namespace OnxAdmin.API.Json;

static class JSON
{
  public static readonly JsonSerializerOptions Options = new()
  {
    Converters = { new EventDataConverter(), new ContentConverter(), new DeltaConverter() },
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public static T Parse<T>(string json, ILogger? logger = null) where T : new()
  {
    try
    {
      return JsonSerializer.Deserialize<T>(json, Options) ?? new T();
    }
    catch (Exception ex) when (ex is JsonException)
    {
      logger?.LogError(ex, "Failed to parse json: {Json}", json);
      return new T();
    }
  }
}