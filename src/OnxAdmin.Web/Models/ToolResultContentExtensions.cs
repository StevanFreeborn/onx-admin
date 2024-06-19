namespace OnxAdmin.Web.Models;

public static class ToolResultContentExtensions
{
  public static T? GetResult<T>(this ToolResultContent content)
  {
    var toolResult = JsonSerializer.Deserialize<ToolResult>(content.Content);
    
    if (toolResult is null)
    {
      return default;
    }
    
    return JsonSerializer.Deserialize<T>(toolResult.Result);
  }
}

record ToolResult
{
  public string Result { get; set; } = string.Empty;
}