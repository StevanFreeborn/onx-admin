using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Utils;

namespace OnxAdmin.AnthropicConnector.Models;

public class Tool
{
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;

  [JsonPropertyName("input_schema")]
  public InputSchema InputSchema { get; set; } = new();

  [JsonConstructor]
  internal Tool()
  {
  }

  public Tool(string name, string description, InputSchema inputSchema)
  {
    ArgumentValidator.ThrowIfNull(name, nameof(name));
    ArgumentValidator.ThrowIfNull(description, nameof(description));
    ArgumentValidator.ThrowIfNull(inputSchema, nameof(inputSchema));

    Name = name;
    Description = description;
    InputSchema = inputSchema;
  }
}