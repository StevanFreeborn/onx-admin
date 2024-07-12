namespace OnxAdmin.API.Options;

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

    if (string.IsNullOrWhiteSpace(options.Key))
    {
      throw new InvalidOperationException("Anthropic key is required");
    }
  }
}