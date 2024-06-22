using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class InputProperty
{
  public string Type { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;

  [JsonConstructor]
  internal InputProperty()
  {
  }

  public InputProperty(string type, string description)
  {
    Type = type;
    Description = description;
  }
}