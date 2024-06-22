namespace OnxAdmin.AnthropicConnector.Utils;

public static class ArgumentValidator
{
  public static void ThrowIfNull<T>(T? value, string name)
  {
    if (value is null)
    {
      throw new ArgumentNullException(name);
    }
  }
}