using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Utils;

namespace OnxAdmin.AnthropicConnector.Models;

public class ChatMessage
{
  public string Role { get; init; } = string.Empty;
  public List<Content> Content { get; init; } = [];

  [JsonConstructor]
  internal ChatMessage()
  {
  }

  public ChatMessage(string role, List<Content> content)
  {
    ArgumentValidator.ThrowIfNull(role, nameof(role));
    ArgumentValidator.ThrowIfNull(content, nameof(content));

    if (MessageRole.IsValidRole(role) is false)
    {
      throw new ArgumentException($"Invalid role: {role}");
    }

    Role = role;
    Content = content;
  }
}