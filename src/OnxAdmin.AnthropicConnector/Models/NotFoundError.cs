using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class NotFoundError : Error
{
  [JsonConstructor]
  internal NotFoundError() : base(ErrorType.NotFoundError)
  {
  }

  public NotFoundError(string message) : base(ErrorType.NotFoundError)
  {
    Message = message;
  }
}