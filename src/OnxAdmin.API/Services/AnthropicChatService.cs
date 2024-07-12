
using System.Diagnostics;

namespace OnxAdmin.API.Services;

class AnthropicChatService(
  IAnthropicApiClient client,
  ILogger<AnthropicChatService> logger,
  ITopicSentryAgent topicSentry
) : IChatService
{
  private readonly IAnthropicApiClient _client = client;
  private readonly ILogger<AnthropicChatService> _logger = logger;
  private readonly ITopicSentryAgent _topicSentry = topicSentry;

  public async Task GenerateResponseAsync(List<Message> messages)
  {
    var mostRecentMessage = messages[^1];
    var mostRecentContent = mostRecentMessage.Content.OfType<TextContent>().FirstOrDefault()?.Text ?? string.Empty;

    var topicResponse = await _topicSentry.ExecuteTaskAsync(mostRecentContent);

    throw new NotImplementedException();
  }
}