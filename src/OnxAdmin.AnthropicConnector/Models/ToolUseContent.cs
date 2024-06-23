namespace OnxAdmin.AnthropicConnector.Models;

public class ToolUseContent : Content
{
  public string Id { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public Dictionary<string, object?> Input { get; set; } = [];

  public ToolUseContent() : base(ContentType.ToolUse)
  {
  }
}