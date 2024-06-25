using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public abstract class Error
{
  public string Type { get; init; } = string.Empty;
  public string Message { get; init; } = string.Empty;

  [JsonConstructor]
  internal Error()
  {
  }

  protected Error(string type)
  {
    Type = type;
  }
}