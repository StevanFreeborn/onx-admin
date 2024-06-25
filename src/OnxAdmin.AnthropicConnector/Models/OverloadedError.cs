using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class OverloadedError : Error
{
  [JsonConstructor]
  public OverloadedError() : base(ErrorType.OverloadedError)
  {
  }

  public OverloadedError(string message) : base(ErrorType.OverloadedError)
  {
    Message = message;
  }
}