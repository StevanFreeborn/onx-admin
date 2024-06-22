using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public abstract class MessageRequest
{
  public bool Stream { get; init; }


  [JsonConstructor]
  protected MessageRequest()
  {
  }

  protected MessageRequest(bool stream)
  {
    Stream = stream;
  }
}