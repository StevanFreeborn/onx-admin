using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public abstract class MessageRequest(bool stream)
{
  public bool Stream { get; init; } = stream;
}