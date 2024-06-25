using System.Text.Json;
using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Models;

namespace OnxAdmin.AnthropicConnector.Json;

class ErrorConverter : JsonConverter<Error>
{
  public override Error Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var jsonDocument = JsonDocument.ParseValue(ref reader);
    var root = jsonDocument.RootElement;
    var type = root.GetProperty("type").GetString();

    return type switch
    {
      ErrorType.InvalidRequestError => JsonSerializer.Deserialize<InvalidRequestError>(root.GetRawText(), options)!,
      ErrorType.AuthenticationError => JsonSerializer.Deserialize<AuthenticationError>(root.GetRawText(), options)!,
      ErrorType.PermissionError => JsonSerializer.Deserialize<PermissionError>(root.GetRawText(), options)!,
      ErrorType.NotFoundError => JsonSerializer.Deserialize<NotFoundError>(root.GetRawText(), options)!,
      ErrorType.RateLimitError => JsonSerializer.Deserialize<RateLimitError>(root.GetRawText(), options)!,
      ErrorType.ApiError => JsonSerializer.Deserialize<ApiError>(root.GetRawText(), options)!,
      ErrorType.OverloadedError => JsonSerializer.Deserialize<OverloadedError>(root.GetRawText(), options)!,
      _ => throw new JsonException($"Unknown error type: {type}")
    };
  }

  public override void Write(Utf8JsonWriter writer, Error value, JsonSerializerOptions options)
  {
    JsonSerializer.Serialize(writer, value, value.GetType(), options);
  }
}