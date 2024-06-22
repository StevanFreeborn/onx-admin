namespace OnxAdmin.AnthropicConnector.Models;

public static class MessageRole
{
  public const string User = "user";
  public const string Assistant = "assistant";
  public static bool IsValidRole(string role) => role is User or Assistant;
}