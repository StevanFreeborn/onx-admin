#pragma warning disable SKEXP0001

using System.IO.Abstractions;

using Tool = Anthropic.SDK.Common.Tool;

namespace OnxAdmin.Web.Services;

class AnthropicOptions
{
  public string Key { get; set; } = string.Empty;
}

class AnthropicOptionsSetup(IConfiguration config) : IConfigureOptions<AnthropicOptions>
{
  private const string SectionName = nameof(AnthropicOptions);
  private readonly IConfiguration _config = config;

  public void Configure(AnthropicOptions options)
  {
    _config.GetSection(SectionName).Bind(options);
  }
}

interface IAnthropicService
{
  IAsyncEnumerable<string> StreamResponseAsync(List<Message> messages);
  Task<MessageResponse> GenerateResponseAsync(List<Message> messages);
}

class AnthropicService(
  HttpClient httpClient, 
  IOptions<AnthropicOptions> options, 
  IOnspringService onspringService,
  ILogger<AnthropicService> logger,
  ISemanticTextMemory memory,
  IWeatherService weatherService,
  IAttachmentService attachmentService
) : ToolProvider, IAnthropicService
{
  private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true };
  private readonly AnthropicClient _client = new(options.Value.Key, httpClient);
  private readonly IOnspringService _onspringService = onspringService;
  private readonly ILogger<AnthropicService> _logger = logger;
  private readonly ISemanticTextMemory _memory = memory;
  private readonly IWeatherService _weatherService = weatherService;
  private readonly IAttachmentService _attachmentService = attachmentService;

  public async IAsyncEnumerable<string> StreamResponseAsync(List<Message> messages)
  {
    var msgParams = new MessageParameters()
    {
      Messages = messages,
      Model = AnthropicModels.Claude3Sonnet,
      MaxTokens = 1024,
      Stream = true,
      Temperature = 0m,
    };

    await foreach (var response in _client.Messages.StreamClaudeMessageAsync(msgParams))
    {
      if (response.Delta is not null)
      {
        yield return response.Delta.Text;
      }
    }
  }

  public async Task<MessageResponse> GenerateResponseAsync(List<Message> messages)
  {
    var mostRecentMessage = messages[^1];
    List<Message> requestMessages = [];

    if (mostRecentMessage.Content.Any(c => c is ToolResultContent))
    {
      requestMessages.AddRange(messages);
    } 
    else
    {
      var previousMessages = messages[..^1];
      var isAboutOnspring = await IsAboutOnspringAsync(mostRecentMessage);
      var knowledge = string.Empty;

      if (isAboutOnspring)
      {
        knowledge = await QueryOnspringKnowledgeAsync(mostRecentMessage);
      }

      var prompt = $@"""
        You are an AI assistant having a conversation with a user. The user's most recent message is:

        <user_message>
        {mostRecentMessage}
        </user_message>

        Here is some additional context that may be relevant to the user's message:

        <context>
        {knowledge}
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

      requestMessages = [..previousMessages, new(RoleType.User, prompt)];
    }

    var tools = new List<Tool>();
    tools.AddRange(_onspringService.GetTools());
    tools.AddRange(_weatherService.GetTools());
    tools.AddRange(GetTools());

    var msgParams = new MessageParameters()
    {
      SystemMessage = """
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
          - If you need to use a tool, provide a clear explanation of why you want to use it and ask for permission.
          - DO NOT assume the user can see the tools output. You MUST provide the output in your response.
        ---

        Use these guidelines to ensure a smooth and efficient experience for users seeking assistance with the Onspring platform.
      """,
      Messages = requestMessages,
      Model = AnthropicModels.Claude3Haiku,
      MaxTokens = 1024,
      Stream = false,
      Temperature = 0m,
    };

    try 
    {
      var response = await _client.Messages.GetClaudeMessageAsync(msgParams, [.. tools]);
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to generate response: {ExceptionMessage}", ex.Message);
      return new MessageResponse()
      {
        Role = RoleType.Assistant,
        Content = [new TextContent() { Text = "I'm sorry, I'm having trouble generating a response right now." }],
      };
    }
  }

  [Function("This tool can be used to perform a data import analysis on a CSV file. It will find the file for the id given, read the file, and provide a summary of the fields needed to import the data into an Onspring app.")]
  public async Task<string> RunImportAnalysisAsync(
    [FunctionParameter("The ID of the CSV file to analyze.", true)]
    string fileId
  )
  {
    var fileExtension = _attachmentService.GetAttachmentExtensionAsync(fileId);

    if (fileExtension != ".csv")
    {
      throw new ToolException("Invalid file format. The file must be a CSV file.");
    }

    var fileContent = await _attachmentService.GetAttachmentLinesAsync(fileId);
    var rows = fileContent.Take(2).ToList();

    var prompt = @$"""
      I will provide you with a CSV file containing data:

      <csv_file>
      {string.Join("\n", rows)}
      </csv_file>

      Your task is to analyze the data in the CSV file and provide a summary of the fields needed to import the data into an Onspring app. The summary should include the following information for each column in the CSV file:
      - The name of the column
      - A brief description of what that column represents
      - The data type of the values in that column

      Here are the steps to complete this task:

      1. Parse the CSV data and identify the column names.
      2. For each column:
        a. Determine the data type by inspecting the values. If the column contains multiple data types, use the most prevalent one.
        b. Select ONLY from the following types:
            1. Date - Use for Date or Date + Time values such as 06-10-2024
            2. List - Use for categorical or label data such as Status, Category, Type, Language, Country, etc.
            4. Text - Use for alphanumeric characters such as HTML markup, json, multi-line text, emails, phone numbers, etc.
            5. Number - Use for any type of numeric value
        b. Come up with a concise description of what the column likely represents based on its name and values.
      3. If you are given 2 rows of data, treat first row as headers and use column names as names. If you are given a single row of data, generate names based on the values in the first row.
      4. Based on the data come up with a name for the app that will hold this data. For example, if the data is about tasks, you could name the app ""Tasks"".
      5. Construct a response that includes a table of the analysis:
        
        | Field Name | Field Description               | Field Type |
        |------------|---------------------------------|------------|
        | name       | The name of the person          | Text       |
        | age        | The age of the person in years  | Number     |
        | city       | The city where the person lives | Text       |
        
      5. Output the table in Markdown format.

      For example, if the input CSV looked like:

      ```
      name,age,city
      Alice,25,New York
      Bob,30,Chicago
      Charlie,35,Houston
      ```

      The expected table summary would be:
      
      | Field Name | Field Description               | Field Type |
      |------------|---------------------------------|------------|
      | name       | The name of the person          | Text       |
      | age        | The age of the person in years  | Number     |
      | city       | The city where the person lives | Text       |

      Remember, ONLY use the types provided: Date, List, Text, Number. DO NOT use any other types.
      Remember, ONLY provide the Table. DO NOT provide any other information.
    """;

    var msgParams = new MessageParameters()
    {
      Messages = [new(RoleType.User, prompt)],
      Model = AnthropicModels.Claude3Haiku,
      MaxTokens = 1024,
      Stream = false,
      Temperature = 0m,
    };

    var response = await _client.Messages.GetClaudeMessageAsync(msgParams);

    return response.Message;
  }

  private async Task<string> QueryOnspringKnowledgeAsync(string query)
  {
    var results = _memory.SearchAsync(
      collection: HelpCenterGenerateEmbeddingsService.HelpCenterCollection,
      query: $"search_query: {query}",
      limit: 10,
      minRelevanceScore: 0
    );

    var knowledge = new StringBuilder();

    await foreach (var result in results)
    {
      var text = result.Metadata.Text.Replace("search_document: ", string.Empty);
      knowledge.AppendLine(text);
    }

    return knowledge.ToString();
  }

  record IsAboutOnspringResponse
  {
    public bool IsAboutOnspring { get; init; } = false;
    public string Reasoning { get; init; } = string.Empty;
  }

  private async Task<bool> IsAboutOnspringAsync(string userInput)
  {
    var prompt = $@"""
      You are an AI assistant that specializes in the Onspring software platform. Your task is to determine whether a given user input is asking about or discussing the Onspring platform.

      Here is the user input:
      <user_input>
      {userInput}
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

    var msgParams = new MessageParameters()
    {
      Messages = [new(RoleType.User, prompt)],
      Model = AnthropicModels.Claude3Haiku,
      MaxTokens = 1024,
      Stream = false,
      Temperature = 0m,
    };

    var response = await _client.Messages.GetClaudeMessageAsync(msgParams);
    var result = Parse<IsAboutOnspringResponse>(response.Message);

    _logger.LogInformation(
      "UserInput: {UserInput} IsAboutOnspring: {IsAboutOnspring}, Reasoning: {Reasoning}",
      userInput, 
      result.IsAboutOnspring, 
      result.Reasoning
    );

    return result.IsAboutOnspring;
  }

  private T Parse<T>(string message) where T : new()
  {
    try
    {
      return JsonSerializer.Deserialize<T>(message, _serializerOptions) ?? new T();
    }
    catch (Exception ex) when (ex is JsonException)
    {
      _logger.LogError(ex, "Failed to parse message: {Message}", message);
      return new T();
    }
  }
}