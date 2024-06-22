namespace OnxAdmin.AnthropicConnector.Models;

public class ToolChoiceType
{
  public const string Auto = "auto";
  public const string Any = "any";
  public const string Tool = "tool";
  internal static bool IsValidType(string type) => type is Auto or Any or Tool;
}