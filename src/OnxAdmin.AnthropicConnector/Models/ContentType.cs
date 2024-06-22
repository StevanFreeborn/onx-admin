namespace OnxAdmin.AnthropicConnector.Models;

public static class ContentType
{
  public const string Text = "text";
  public const string Image = "image";
  public static bool IsValidContentType(string contentType) => contentType is Text or Image;
}