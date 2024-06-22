namespace OnxAdmin.AnthropicConnector.Models;

public static class ImageType
{
  public const string Jpg = "image/jpeg";
  public const string Png = "image/png";
  public const string Gif = "image/gif";
  public const string Webp = "image/webp";
  public static bool IsValidImageType(string imageType) => imageType is Jpg or Png or Gif or Webp;
}