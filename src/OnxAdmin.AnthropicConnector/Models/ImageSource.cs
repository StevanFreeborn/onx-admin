using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class ImageSource
{
  public string MediaType { get; init; } = string.Empty;
  public string Data { get; init; } = string.Empty;

  [JsonConstructor]
  internal ImageSource()
  {
  }

  public ImageSource(string mediaType, string data)
  {
    ArgumentNullException.ThrowIfNull(mediaType, nameof(mediaType));

    if (ImageType.IsValidImageType(mediaType) is false)
    {
      throw new ArgumentException($"Invalid media type: {mediaType}");
    }

    ArgumentNullException.ThrowIfNull(data, nameof(data));

    MediaType = mediaType;
    Data = data;
  }
}