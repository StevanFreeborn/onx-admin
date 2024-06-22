using System.Text.Json;

namespace OnxAdmin.AnthropicConnector.Json;

public static class JsonSerializationOptions
{
  public static JsonSerializerOptions DefaultOptions => new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters =
    {
      new ContentConverter(),
      new ToolChoiceConverter()
    }
  };
}