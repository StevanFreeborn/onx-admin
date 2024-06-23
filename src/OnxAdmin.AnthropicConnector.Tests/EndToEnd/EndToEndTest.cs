namespace OnxAdmin.AnthropicConnector.Tests.EndToEnd;

public class EndToEndTest(
  HttpClientFixture httpClientFixture,
  ConfigurationFixture configFixture
) : IClassFixture<HttpClientFixture>, IClassFixture<ConfigurationFixture>
{
  protected readonly AnthropicClient _client = new(configFixture.AnthropicApiKey, httpClientFixture.HttpClient);
}