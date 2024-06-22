using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Utils;

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
    ArgumentValidator.ThrowIfNull(type, nameof(type));
    ArgumentValidator.ThrowIfNull(description, nameof(description));

    Type = type;
    Description = description;
  }
}