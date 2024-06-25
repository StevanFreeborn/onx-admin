using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class ChatResponse
{
  public string Id { get; set; } = string.Empty;
  public string Model { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;

  [JsonPropertyName("stop_reason")]
  public string StopReason { get; set; } = string.Empty;

  [JsonPropertyName("stop_sequence")]
  public string StopSequence { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public ChatUsage Usage { get; set; } = new();
  public List<Content> Content { get; set; } = [];
}