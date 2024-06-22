using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public abstract class Content
{
  public string Type { get; init; } = string.Empty;
  
  [JsonConstructor]
  internal Content()
  {
  }

  protected Content(string type)
  {
    ArgumentNullException.ThrowIfNull(type, nameof(type));
    
    if (ContentType.IsValidContentType(type) is false)
    {
      throw new ArgumentException($"Invalid content type: {type}");
    }
    
    Type = type;
  }
}