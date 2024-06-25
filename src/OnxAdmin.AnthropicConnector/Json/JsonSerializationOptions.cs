using System.Text.Json;
using System.Text.Json.Serialization;

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
      new ToolChoiceConverter(),
      new ErrorConverter(),
    },
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
  };
}