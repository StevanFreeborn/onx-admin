using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Utils;

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
    ArgumentValidator.ThrowIfNull(mediaType, nameof(mediaType));
    ArgumentValidator.ThrowIfNull(data, nameof(data));

    Source = new(mediaType, data);
  }
}