using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Utils;

namespace OnxAdmin.AnthropicConnector.Models;

public class SpecificToolChoice : ToolChoice
{
  public string Name { get; init; } = string.Empty;

  [JsonConstructor]
  internal SpecificToolChoice() : base(ToolChoiceType.Tool)
  {
  }

  public SpecificToolChoice(string name) : base(ToolChoiceType.Tool)
  {
    ArgumentValidator.ThrowIfNull(name, nameof(name));

    Name = name;
  }
}