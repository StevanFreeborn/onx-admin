using OnxAdmin.AnthropicConnector.Models;

public class AnthropicResult<T>
{
  public T Value { get; }
  public AnthropicError Error { get; }
  public bool IsSuccess { get; }
  public string RequestId { get; }

  protected AnthropicResult(T value, AnthropicError error, bool isSuccess, string requestId)
  {
    Value = value;
    Error = error;
    IsSuccess = isSuccess;
    RequestId = requestId;
  }

  public static AnthropicResult<T> Success(T value, string requestId)
  {
    return new AnthropicResult<T>(value, null!, true, requestId);
  }

  public static AnthropicResult<T> Failure(AnthropicError error, string requestId)
  {
    return new AnthropicResult<T>(default!, error, false, requestId);
  }
}
