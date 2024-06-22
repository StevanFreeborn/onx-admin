using System.Text.Json.Serialization;

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
    ArgumentNullException.ThrowIfNull(name, nameof(name));
    
    Name = name;
  }
}