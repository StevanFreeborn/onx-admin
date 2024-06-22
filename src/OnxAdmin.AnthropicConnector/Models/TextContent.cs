using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Utils;

namespace OnxAdmin.AnthropicConnector.Models;

public class TextContent : Content
{
  public string Text { get; init; } = string.Empty;

  [JsonConstructor]
  internal TextContent() : base(ContentType.Text)
  {
  }

  public TextContent(string text) : base(ContentType.Text)
  {
    ArgumentValidator.ThrowIfNull(text, nameof(text));

    Text = text;
  }
}