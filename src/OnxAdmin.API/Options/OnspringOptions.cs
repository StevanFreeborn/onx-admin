namespace OnxAdmin.API.Options;

class OnspringOptions
{
  public string InstanceUrl { get; set; } = string.Empty;
  public string CopilotUsername { get; set; } = string.Empty;
  public string CopilotPassword { get; set; } = string.Empty;
}

class OnspringOptionsSetup(IConfiguration config) : IConfigureOptions<OnspringOptions>
{
  private const string SectionName = nameof(OnspringOptions);
  private readonly IConfiguration _config = config;

  public void Configure(OnspringOptions options)
  {
    _config.GetSection(SectionName).Bind(options);

    if (
      string.IsNullOrWhiteSpace(options.InstanceUrl) ||
      Uri.TryCreate(options.InstanceUrl, UriKind.Absolute, out var uri) is false ||
      uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps
    )
    {
      throw new ArgumentException("OllamaTextEmbeddingOptions: BaseUrl must be a valid HTTP or HTTPS URL");
    }

    if (string.IsNullOrWhiteSpace(options.CopilotUsername))
    {
      throw new ArgumentException("OnspringOptions: CopilotUsername is required");
    }

    if (string.IsNullOrWhiteSpace(options.CopilotPassword))
    {
      throw new ArgumentException("OnspringOptions: CopilotPassword is required");
    }
  }
}