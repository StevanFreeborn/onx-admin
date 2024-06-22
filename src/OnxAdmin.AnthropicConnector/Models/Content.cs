using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public abstract class Content
{
  public string Type { get; init; } = string.Empty;

  [JsonConstructor]
  internal Content()
  {
  }

  protected Content(string type)
  {
    Type = type;
  }
}