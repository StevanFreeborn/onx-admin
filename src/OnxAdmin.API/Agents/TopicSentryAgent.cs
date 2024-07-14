

namespace OnxAdmin.API.Agents;

interface ITopicSentryAgent
{
  Task<TopicResponse> ExecuteTaskAsync(string input);
}

class TopicSentryAgent(
  IAnthropicApiClient client,
  ILogger<TopicSentryAgent> logger,
  Instrumentation instrumentation
) : ITopicSentryAgent
{
  private readonly IAnthropicApiClient _client = client;
  private readonly ILogger<TopicSentryAgent> _logger = logger;
  private readonly ActivitySource _activitySource = instrumentation.ActivitySource;

  public async Task<TopicResponse> ExecuteTaskAsync(string input)
  {
    using var activity = _activitySource.StartActivity(nameof(ExecuteTaskAsync));
    activity?.SetTag("input", input);

    var prompt = GeneratePrompt(input);
    var request = new MessageRequest(
      AnthropicModels.Claude35Sonnet,
      [new(MessageRole.User, [new TextContent(prompt)])]
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

    activity?.SetTag("output", JsonSerializer.Serialize(topicResponse, JSON.Options));

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
  public bool IsAboutOnspring { get; init; }
  public string Reasoning { get; init; }

  public TopicResponse() : this(false, string.Empty) { }

  public TopicResponse(bool isAboutOnspring, string reasoning)
  {
    IsAboutOnspring = isAboutOnspring;
    Reasoning = reasoning;
  }
}