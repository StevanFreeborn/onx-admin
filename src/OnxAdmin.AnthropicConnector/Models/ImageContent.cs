using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class ImageContent : Content
{
  public ImageSource Source { get; init; } = new();

  [JsonConstructor]
  internal ImageContent()
  {
  }
  
  public ImageContent(string mediaType, string data) : base(ContentType.Image)
  {
    ArgumentNullException.ThrowIfNull(mediaType, nameof(mediaType));
    ArgumentNullException.ThrowIfNull(data, nameof(data));
    
    Source = new(mediaType, data);
  }
}