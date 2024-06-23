using System.Text.Json.Serialization;

namespace OnxAdmin.AnthropicConnector.Models;

public class ChatUsage
{
  [JsonPropertyName("input_tokens")]
  public int InputTokens { get; set; }

  [JsonPropertyName("output_tokens")]
  public int OutputTokens { get; set; }
}