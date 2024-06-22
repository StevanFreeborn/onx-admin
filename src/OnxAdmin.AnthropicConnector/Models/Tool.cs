using System.Text.Json.Serialization;

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
    ArgumentNullException.ThrowIfNull(name, nameof(name));
    ArgumentNullException.ThrowIfNull(description, nameof(description));
    ArgumentNullException.ThrowIfNull(inputSchema, nameof(inputSchema));

    Name = name;
    Description = description;
    InputSchema = inputSchema;
  }
}