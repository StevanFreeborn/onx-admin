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
  IAsyncEnumerable<string> GenerateResponseAsync(List<Message> messages);
}

class AnthropicService(HttpClient httpClient, IOptions<AnthropicOptions> options) : IAnthropicService
{
  private readonly AnthropicClient _client = new(options.Value.Key, httpClient);

  public async IAsyncEnumerable<string> GenerateResponseAsync(List<Message> messages)
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
}