

namespace OnxAdmin.API.Agents.TopicSentry;

interface ITopicSentryAgent
{
  Task<TopicResponse> ExecuteTaskAsync(string input);
}

class TopicSentryAgent(
  IAnthropicApiClient client,
  ILogger<TopicSentryAgent> logger
) : ITopicSentryAgent
{
  private readonly IAnthropicApiClient _client = client;
  private readonly ILogger<TopicSentryAgent> _logger = logger;

  public async Task<TopicResponse> ExecuteTaskAsync(string input)
  {
    var prompt = GeneratePrompt(input);
    var request = new MessageRequest(
      AnthropicModels.Claude35Sonnet,
      [new(MessageRole.User, [new TextContent(input)])]
    );

    var response = await _client.CreateMessageAsync(request);
    TopicResponse topicResponse;

    if (response.IsFailure)
    {
      _logger.LogError("Failed to generate response: {Error}", response.Error);
      topicResponse = new TopicResponse(false, "Failed to determine if input is about Onspring");
    }
    else
    {
      var text = response.Value.Content.OfType<TextContent>().FirstOrDefault()?.Text ?? string.Empty;
      topicResponse = JSON.Parse<TopicResponse>(text, _logger);
    }

    _logger.LogInformation("Topic Sentry response: {Response}", topicResponse);

    return topicResponse;
  }

  private static string GeneratePrompt(string input)
  {
    return $@"""
      You are an AI assistant that specializes in the Onspring software platform. Your task is to determine whether a given user input is asking about or discussing the Onspring platform.

      Here is the user input:
      <user_input>
      {input}
      </user_input>

      First, analyze the input and provide your reasoning for whether it is or is not about Onspring.

      Then, provide a true or false answer indicating if the input is about Onspring.

      You should provide your answer as a JSON object like the following:

      {{
        ""isAboutOnspring"": true,
        ""reasoning"": ""An explanation of why you think it is""
      }}

      Some key things to consider:
      - Onspring is a no-code software platform for building business applications and workflows
      - Onspring has features like forms, fields, records, reports, dashboards, etc.
      - Onspring is often used for processes like audit, risk, compliance, legal, HR and more
      - Onspring is highly configurable and customizable to each client's needs
      - If the input mentions Onspring by name, it is definitely about Onspring

      Remember you should output ONLY your answer as a JSON object.

      DO NOT output anything else.
    """;
  }
}

record TopicResponse
{
  public bool IsAboutOnspring { get; init; } = false;
  public string Reasoning { get; init; } = string.Empty;

  public TopicResponse() { }

  public TopicResponse(bool isAboutOnspring, string reasoning)
  {
    IsAboutOnspring = isAboutOnspring;
    Reasoning = reasoning;
  }
}