using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class AuthenticationError : Error
{
  [JsonConstructor]
  internal AuthenticationError() : base(ErrorType.AuthenticationError)
  {
  }

  public AuthenticationError(string message) : base(ErrorType.AuthenticationError)
  {
    Message = message;
  }
}