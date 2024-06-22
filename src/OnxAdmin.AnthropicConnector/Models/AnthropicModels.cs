namespace OnxAdmin.AnthropicConnector.Models;

public static class AnthropicModels
{
  public const string Claude3Opus = "claude-3-opus-20240229";
  public const string Claude3Sonnet = "claude-3-sonnet-20240229";
  public const string Claude35Sonnet = "claude-3-5-sonnet-20240620";
  public const string Claude3Haiku = "claude-3-haiku-20240307";
  public static bool IsValidModel(string modelId) => modelId is Claude3Opus or Claude3Sonnet or Claude35Sonnet or Claude3Haiku;
}