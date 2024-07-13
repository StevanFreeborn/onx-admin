namespace OnxAdmin.API.Extensions;

public static class MessageExtensions
{
  public static string GetText(this Message message)
  {
    return message.Content.OfType<TextContent>().FirstOrDefault()?.Text ?? string.Empty;
  }
}