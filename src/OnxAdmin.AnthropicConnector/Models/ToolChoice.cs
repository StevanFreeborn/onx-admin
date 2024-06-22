using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public abstract class ToolChoice
{
  public string Type { get; init; } = string.Empty;

  [JsonConstructor]
  protected ToolChoice()
  {
  }

  protected ToolChoice(string type)
  {
    Type = type;
  }
}