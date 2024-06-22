using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class InputSchema
{
  public string Type { get; init; } = "object";
  public Dictionary<string, InputProperty> Properties { get; init; } = [];
  public List<string> Required { get; init; } = [];

  [JsonConstructor]
  internal InputSchema()
  {
  }

  public InputSchema(Dictionary<string, InputProperty> properties, List<string> required)
  {
    ArgumentNullException.ThrowIfNull(properties, nameof(properties));
    ArgumentNullException.ThrowIfNull(required, nameof(required));

    if (required.Any(r => properties.ContainsKey(r) is false))
    {
      throw new ArgumentException("Required properties must be present in the properties dictionary.");
    }

    Properties = properties;
    Required = required;
  }
}