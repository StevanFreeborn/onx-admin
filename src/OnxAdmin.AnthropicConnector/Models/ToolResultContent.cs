using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Utils;

namespace OnxAdmin.AnthropicConnector.Models;

public class ToolResultContent : Content
{
  [JsonPropertyName("tool_use_id")]
  public string ToolUseId { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;

  [JsonConstructor]
  internal ToolResultContent() : base(ContentType.ToolResult) { }

  public ToolResultContent(string toolUseId, string content) : base(ContentType.ToolResult)
  {
    ArgumentValidator.ThrowIfNull(toolUseId, nameof(toolUseId));
    ArgumentValidator.ThrowIfNull(content, nameof(content));

    ToolUseId = toolUseId;
    Content = content;
  }
}