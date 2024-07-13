namespace OnxAdmin.API.Options;

class ChromaOptions
{
  public string BaseUrl { get; set; } = string.Empty;
}

class ChromaOptionsSetup(IConfiguration config) : IConfigureOptions<ChromaOptions>
{
  private const string SectionName = nameof(ChromaOptions);
  private readonly IConfiguration _config = config;

  public void Configure(ChromaOptions options)
  {
    _config.GetSection(SectionName).Bind(options);

    if (
      string.IsNullOrWhiteSpace(options.BaseUrl) ||
      Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri) is false ||
      uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps
    )
    {
      throw new ArgumentException("OllamaTextEmbeddingOptions: BaseUrl must be a valid HTTP or HTTPS URL");
    }
  }
}