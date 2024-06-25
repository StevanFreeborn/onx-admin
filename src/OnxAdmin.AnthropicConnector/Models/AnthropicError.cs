using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class AnthropicError
{
  public string Type { get; init; } = "error";
  public Error? Error { get; init; } = null;

  [JsonConstructor]
  internal AnthropicError()
  {
  }

  public AnthropicError(Error error)
  {
    Error = error;
  }
}