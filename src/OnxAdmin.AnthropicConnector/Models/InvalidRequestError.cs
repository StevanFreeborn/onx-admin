using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class InvalidRequestError : Error
{
  [JsonConstructor]
  internal InvalidRequestError() : base(ErrorType.InvalidRequestError)
  {
  }

  public InvalidRequestError(string message) : base(ErrorType.InvalidRequestError)
  {
    Message = message;
  }
}