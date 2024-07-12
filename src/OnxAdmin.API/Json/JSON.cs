namespace OnxAdmin.API.Json;

static class JSON
{
  private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

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