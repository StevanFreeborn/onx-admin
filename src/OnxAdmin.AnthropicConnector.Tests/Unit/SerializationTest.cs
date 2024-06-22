using OnxAdmin.AnthropicConnector.Json;

namespace OnxAdmin.AnthropicConnector.Tests.Unit;

public class SerializationTest
{
  private readonly JsonSerializerOptions _jsonSerializerOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters = { new ContentConverter() }
  };

  protected string Serialize<T>(T obj) => JsonSerializer.Serialize(obj, _jsonSerializerOptions);

  protected T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
}