namespace OnxAdmin.AnthropicConnector.Tests.EndToEnd;

public class HttpClientFixture
{
  public HttpClient HttpClient { get; }

  public HttpClientFixture()
  {
    HttpClient = new HttpClient();
  }
}