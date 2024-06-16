class GroqOptions
{
  public string BaseUrl { get; set; } = string.Empty;
  public string ApiKey { get; set; } = string.Empty;
}

class GroqOptionsSetup(IConfiguration config) : IConfigureOptions<GroqOptions>
{
  private const string SectionName = nameof(GroqOptions);
  private readonly IConfiguration _config = config;

  public void Configure(GroqOptions options)
  {
    _config.GetSection(SectionName).Bind(options);
  }
}