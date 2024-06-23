namespace OnxAdmin.AnthropicConnector.Tests.EndToEnd;

public class ConfigurationFixture
{
  public string AnthropicApiKey { get; }

  public ConfigurationFixture()
  {
    var configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.Test.json")
      .AddEnvironmentVariables()
      .Build();

    var anthropicApiKey = configuration["AnthropicApiKey"];

    if (string.IsNullOrEmpty(anthropicApiKey))
    {
      throw new InvalidOperationException("AnthropicApiKey is required");
    }

    AnthropicApiKey = anthropicApiKey;
  }
}