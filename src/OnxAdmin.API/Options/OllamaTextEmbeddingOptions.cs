namespace OnxAdmin.API.Options;

class OllamaTextEmbeddingOptions
{
  public string BaseUrl { get; set; } = string.Empty;
  public string ModelId { get; set; } = string.Empty;
}

class OllamaTextEmbeddingOptionsSetup(IConfiguration config) : IConfigureOptions<OllamaTextEmbeddingOptions>
{
  private const string SectionName = nameof(OllamaTextEmbeddingOptions);
  private readonly IConfiguration _config = config;

  public void Configure(OllamaTextEmbeddingOptions options)
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

    if (string.IsNullOrWhiteSpace(options.ModelId))
    {
      throw new ArgumentException("OllamaTextEmbeddingOptions: ModelId is required");
    }
  }
}