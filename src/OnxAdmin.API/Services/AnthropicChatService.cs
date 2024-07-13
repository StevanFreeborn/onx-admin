namespace OnxAdmin.API.Services;

class AnthropicChatService(
  ILogger<AnthropicChatService> logger,
  ITopicSentryAgent topicSentry,
  IOnspringResearcherAgent onspringResearcher,
  IOnspringAdministratorAgent onspringAdministrator
) : IChatService
{
  private readonly ILogger<AnthropicChatService> _logger = logger;
  private readonly ITopicSentryAgent _topicSentry = topicSentry;
  private readonly IOnspringResearcherAgent _onspringResearcher = onspringResearcher;
  private readonly IOnspringAdministratorAgent _onspringAdministrator = onspringAdministrator;

  public async Task<Message> GenerateResponseAsync(List<Message> messages)
  {
    var messagesToReturn = messages.ToList();
    var previousMessages = messagesToReturn[..^1];
    var mostRecentMessage = messagesToReturn[^1];
    var mostRecentMessageText = mostRecentMessage.GetText();
    var topicResponse = await _topicSentry.ExecuteTaskAsync(mostRecentMessageText);
    var knowledge = topicResponse.IsAboutOnspring
      ? await _onspringResearcher.ExecuteTaskAsync(mostRecentMessageText)
      : [];
    return await _onspringAdministrator.ExecuteTaskAsync(mostRecentMessageText, previousMessages, knowledge);
  }
}