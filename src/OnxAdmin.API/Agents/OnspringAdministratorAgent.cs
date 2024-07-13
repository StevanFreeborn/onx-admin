namespace OnxAdmin.API.Agents;

interface IOnspringAdministratorAgent
{
  Task<Message> ExecuteTaskAsync(string mostRecentMessage, List<Message> previousMessages, List<Finding> knowledge);
}

class OnspringAdministratorAgent(
  IAnthropicApiClient anthropicApiClient,
  ILogger<OnspringAdministratorAgent> logger,
  Instrumentation instrumentation
) : IOnspringAdministratorAgent
{
  private readonly IAnthropicApiClient _anthropicApiClient = anthropicApiClient;
  private readonly ILogger<OnspringAdministratorAgent> _logger = logger;
  private readonly ActivitySource _activitySource = instrumentation.ActivitySource;
  private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, Converters = { new ContentConverter() } };

  public async Task<Message> ExecuteTaskAsync(string mostRecentMessage, List<Message> previousMessages, List<Finding> knowledge)
  {
    using var activity = _activitySource.StartActivity(nameof(ExecuteTaskAsync));
    activity?.SetTag("input.previousMessages", JsonSerializer.Serialize(previousMessages, _jsonSerializerOptions));
    activity?.SetTag("input.knowledge", JsonSerializer.Serialize(knowledge, _jsonSerializerOptions));
    activity?.SetTag("input.mostRecentMessage", mostRecentMessage);

    var prompt = GeneratePrompt(mostRecentMessage, knowledge);
    var request = new MessageRequest(
      AnthropicModels.Claude35Sonnet,
      [
        ..previousMessages,
        new(MessageRole.User, [new TextContent(prompt)])
      ],
      system: SystemMessage
    );

    var result = await _anthropicApiClient.CreateMessageAsync(request);

    Message msg;

    if (result.IsFailure)
    {
      _logger.LogError("Failed to generate response: {Error}", result.Error);

      msg = new Message(
        MessageRole.Assistant,
        [new TextContent("I'm sorry, I'm having trouble generating a response right now.")]
      );
    }
    else
    {
      msg = new Message(result.Value.Role, result.Value.Content);
    }

    activity?.SetTag("output", msg.GetText());

    return msg;
  }

  private static string GeneratePrompt(string mostRecentMessage, List<Finding> knowledge)
  {
    return $@"""
        You are an AI assistant having a conversation with a user. The user's most recent message is:

        <user_message>
        {mostRecentMessage}
        </user_message>

        Here is some additional context that may be relevant to the user's message:

        <context>
        {knowledge.Aggregate(new StringBuilder(), (sb, finding) => sb.AppendLine(finding.Content), sb => sb.ToString())}
        </context>

        The user DOES NOT know that this additional context has been provided. DO NOT reference the context directly in your response. Only use it to inform your response.

        Carefully review the full conversation history and the additional context provided. 

        <scratchpad>
        Think through how the additional context relates to the user's most recent message and the overall conversation. Consider what would be the most helpful response to provide, given the chat history and the context. 
        </scratchpad>

        Provide the best possible response to the user's message, taking into account the entire conversation and the additional context.

        YOU SHOULD write your response as if you were responding to the user directly. 
        
        DO NOT reference the additional context in your response.
      """;
  }

  private const string SystemMessage = """
    You are a helpful and knowledgeable administrator of the Onspring platform. Your primary job is to assist users to the best of your ability using your knowledge of the platform.

    ### Guidelines:

    1. **Assistance:**
      - Provide helpful and informative responses to users.
      - Utilize your knowledge of the Onspring platform effectively.
      - If you are unsure of what the user is asking, ask for clarification.
      - Never make up information or provide incorrect answers.

    2. **Communication:**
      - Be polite and professional in all interactions.
      - Format your responses using Markdown for better readability.

    3. **Tool Usage:**
      - Do not reference any of the tools or functions available to you in your responses.
      - You may use tools when explicitly prompted to do so.
      - If you need to use a tool, provide a clear explanation of why you want to use it and ASK FOR PERMISSION.
      - DO NOT assume the user can see the tool result. You MUST provide the tool result in your response.
    ---

    Use these guidelines to ensure a smooth and efficient experience for users seeking assistance with the Onspring platform.
  """;
}