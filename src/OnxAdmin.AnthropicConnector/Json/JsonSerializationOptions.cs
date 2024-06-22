using System.Text.Json;

namespace OnxAdmin.AnthropicConnector.Json;

static class JsonSerializationOptions
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