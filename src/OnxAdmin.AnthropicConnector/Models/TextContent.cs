using System.Text.Json.Serialization;

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
    ArgumentNullException.ThrowIfNull(text, nameof(text));
    
    Text = text;
  }
}