namespace OnxAdmin.AnthropicConnector.Models;

public class ErrorType
{
  public const string InvalidRequestError = "invalid_request_error";
  public const string AuthenticationError = "authentication_error";
  public const string PermissionError = "permission_error";
  public const string NotFoundError = "not_found_error";
  public const string RateLimitError = "rate_limit_error";
  public const string ApiError = "api_error";
  public const string OverloadedError = "overloaded_error";
}