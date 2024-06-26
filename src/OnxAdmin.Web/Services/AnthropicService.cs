using System.Reflection;

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

class AnthropicService(HttpClient httpClient, IOptions<AnthropicOptions> options, IOnspringService onspringService) : IAnthropicService
{
  private readonly AnthropicClient _client = new(options.Value.Key, httpClient);
  private readonly IOnspringService _onspringService = onspringService;

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
    var tools = new List<Tool>
    {
      Tool.FromFunc("Get_Weather", ([FunctionParameter("Location of the weather", true)] string location) => "72 degrees and sunny"),
    };

    var onspringTools = _onspringService.GetTools();

    tools.AddRange(onspringTools);

    var msgParams = new MessageParameters()
    {
      SystemMessage = """
        You are a helpful and knowledge administrator of the Onspring platform. 
        Your job is to assist the user to the best of your ability using the knowledge you have about the platform and the tools you have available to you. 
        You should write your responses in a way that is helpful and informative to the user. 
        You should also be polite and professional in your responses.
        You should format your responses using Markdown to make them easier to read.
      """,
      Messages = messages,
      Model = AnthropicModels.Claude3Haiku,
      MaxTokens = 1024,
      Stream = false,
      Temperature = 0m,
    };

    var response = await _client.Messages.GetClaudeMessageAsync(msgParams, [.. tools]);

    return response;
  }
}