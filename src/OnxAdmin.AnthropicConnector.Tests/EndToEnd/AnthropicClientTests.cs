namespace OnxAdmin.AnthropicConnector.Tests.EndToEnd;

public class AnthropicClientTests(
  HttpClientFixture httpClientFixture,
  ConfigurationFixture configFixture
) : EndToEndTest(httpClientFixture, configFixture)
{
  [Fact]
  public async Task CreateChatMessage_WhenCalled_ShouldReturnChatResponse()
  {
    var request = new ChatMessageRequest(
      model: AnthropicModels.Claude3Haiku,
      messages: [new(MessageRole.User, [new TextContent("Hello!")])]
    );

    var result = await _client.CreateChatMessageAsync(request);

    result.IsSuccess.Should().BeTrue();
    result.Value.Should().BeOfType<ChatResponse>();
  }
}