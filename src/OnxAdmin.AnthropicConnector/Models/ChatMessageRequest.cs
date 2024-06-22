using System.Text.Json.Serialization;

using OnxAdmin.AnthropicConnector.Utils;

namespace OnxAdmin.AnthropicConnector.Models;

public class ChatMessageRequest : MessageRequest
{
  public string Model { get; init; } = string.Empty;
  public string? System { get; init; } = null;
  public List<ChatMessage> Messages { get; init; } = [];

  [JsonPropertyName("max_tokens")]
  public int MaxTokens { get; init; } = 1024;
  public Dictionary<string, object>? Metadata { get; init; } = null;

  [JsonPropertyName("stop_sequences")]
  public List<string> StopSequences { get; init; } = [];
  public decimal Temperature { get; init; } = 0.0m;
  public int? TopK { get; init; } = null;
  public decimal? TopP { get; init; } = null;

  [JsonPropertyName("tool_choice")]
  public ToolChoice? ToolChoice { get; init; } = null;
  public List<Tool>? Tools { get; init; } = null;

  [JsonConstructor]
  internal ChatMessageRequest() : base(false) { }

  public ChatMessageRequest(
    string model,
    List<ChatMessage> messages,
    int maxTokens = 1024,
    string? system = null,
    Dictionary<string, object>? metadata = null,
    decimal temperature = 0.0m,
    int? topK = null,
    decimal? topP = null,
    ToolChoice? toolChoice = null,
    List<Tool>? tools = null
  ) : base(false)
  {
    ArgumentValidator.ThrowIfNull(model, nameof(model));
    ArgumentValidator.ThrowIfNull(messages, nameof(messages));

    if (AnthropicModels.IsValidModel(model) is false)
    {
      throw new ArgumentException($"Invalid model ID: {model}");
    }

    if (messages.Count < 1)
    {
      throw new ArgumentException("Messages must contain at least one message");
    }

    if (maxTokens < 1)
    {
      throw new ArgumentException($"Invalid max tokens: {maxTokens}");
    }

    if (temperature < 0.0m || temperature > 1.0m)
    {
      throw new ArgumentException($"Invalid temperature: {temperature}");
    }

    Model = model;
    Messages = messages;
    MaxTokens = maxTokens;
    System = system;
    Metadata = metadata;
    Temperature = temperature;
    TopK = topK;
    TopP = topP;
    ToolChoice = toolChoice;
    Tools = tools;
  }
}