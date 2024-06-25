using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class PermissionError : Error
{
  [JsonConstructor]
  internal PermissionError() : base(ErrorType.PermissionError)
  {
  }

  public PermissionError(string message) : base(ErrorType.PermissionError)
  {
    Message = message;
  }
}