using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class ApiError : Error
{
  [JsonConstructor]
  public ApiError() : base(ErrorType.ApiError)
  {
  }

  public ApiError(string message) : base(ErrorType.ApiError)
  {
    Message = message;
  }
}