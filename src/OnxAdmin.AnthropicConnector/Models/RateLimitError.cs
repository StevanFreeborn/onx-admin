using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models
{
  public class RateLimitError : Error
  {
    [JsonConstructor]
    internal RateLimitError() : base(ErrorType.RateLimitError)
    {
    }

    public RateLimitError(string message) : base(ErrorType.RateLimitError)
    {
      Message = message;
    }
  }
}