namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ChatMessageRequestTests : SerializationTest
{
  [Fact]
  public void Constructor_WhenCalled_ItShouldInitializeProperties()
  {
    var model = AnthropicModels.Claude3Sonnet;
    var messages = new List<ChatMessage> { new() };
    var maxTokens = 512;
    var system = "test-system";
    var metadata = new { test = "test" };
    var temperature = 0.5m;
    var topK = 10;
    var topP = 0.5m;
    var toolChoice = new AutoToolChoice();
    var tools = new List<Tool> { new() };

    var chatMessageRequest = new ChatMessageRequest(
      model: model,
      messages: messages,
      maxTokens: maxTokens,
      system: system,
      metadata: metadata,
      temperature: temperature,
      topK: topK,
      topP: topP,
      toolChoice: toolChoice,
      tools: tools
    );

    chatMessageRequest.Model.Should().Be(model);
    chatMessageRequest.Messages.Should().BeSameAs(messages);
    chatMessageRequest.MaxTokens.Should().Be(maxTokens);
    chatMessageRequest.System.Should().Be(system);
    chatMessageRequest.Metadata.Should().BeSameAs(metadata);
    chatMessageRequest.Temperature.Should().Be(temperature);
    chatMessageRequest.TopK.Should().Be(topK);
    chatMessageRequest.TopP.Should().Be(topP);
    chatMessageRequest.ToolChoice.Should().Be(toolChoice);
    chatMessageRequest.Tools.Should().BeSameAs(tools);
  }

  [Fact]
  public void Constructor_WhenCalledAndModelIsNull_ItShouldThrowArgumentNullException()
  {
    var action = () => new ChatMessageRequest(
      model: null!,
      messages: [new()]
    );

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WhenCalledAndMessagesIsNull_ItShouldThrowArgumentNullException()
  {
    var action = () => new ChatMessageRequest(
      model: AnthropicModels.Claude3Sonnet,
      messages: null!
    );

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WhenCalledAndModelIsInvalid_ItShouldThrowArgumentException()
  {
    var action = () => new ChatMessageRequest(
      model: "invalid-model",
      messages: [new()]
    );

    action.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void Constructor_WhenCalledAndMessagesIsEmpty_ItShouldThrowArgumentException()
  {
    var action = () => new ChatMessageRequest(
      model: AnthropicModels.Claude3Sonnet,
      messages: []
    );

    action.Should().Throw<ArgumentException>();
  }
}