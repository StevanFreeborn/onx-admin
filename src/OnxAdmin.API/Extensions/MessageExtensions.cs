namespace OnxAdmin.API.Extensions;

static class MessageExtensions
{
  public static string GetText(this Message message)
  {
    return message.Content.OfType<TextContent>().FirstOrDefault()?.Text ?? string.Empty;
  }
}

static class MessageResponseExtensions
{
  public static string GetText(this MessageResponse messageResponse)
  {
    return messageResponse.Content.OfType<TextContent>().FirstOrDefault()?.Text ?? string.Empty;
  }
}