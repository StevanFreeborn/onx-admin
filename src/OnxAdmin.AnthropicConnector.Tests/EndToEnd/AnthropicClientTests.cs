namespace OnxAdmin.AnthropicConnector.Tests.EndToEnd;

public class AnthropicClientTests(
  HttpClientFixture httpClientFixture,
  ConfigurationFixture configFixture
) : EndToEndTest(httpClientFixture, configFixture)
{
}