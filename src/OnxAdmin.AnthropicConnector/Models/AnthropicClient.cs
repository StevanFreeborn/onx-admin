using OnxAdmin.AnthropicConnector.Http;

namespace OnxAdmin.AnthropicConnector.Models;

interface IAnthropicClient
{
}

public class AnthropicClient : IAnthropicClient
{
  private const string BaseUrl = "https://api.anthropic.com/v1";
  private const string ApiKeyHeader = "x-api-key";
  private readonly Dictionary<string, string> _defaultHeaders = new()
  {
    { "anthropic-version", "2023-06-01" },
    { "Content-Type", "application/json" }
  };
  private readonly HttpClient _httpClient;

  public AnthropicClient(string apiKey, HttpClient? httpClient = null)
  {
    ArgumentNullException.ThrowIfNull(apiKey, nameof(apiKey));

    _httpClient = httpClient ?? HttpClientFactory.GetHttpClient(BaseUrl);
    _httpClient.DefaultRequestHeaders.Add(ApiKeyHeader, apiKey);
    
    foreach (var (key, value) in _defaultHeaders)
    {
      _httpClient.DefaultRequestHeaders.Add(key, value);
    }
  }
}