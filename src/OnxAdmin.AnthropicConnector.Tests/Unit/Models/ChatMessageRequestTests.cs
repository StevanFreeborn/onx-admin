namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ChatMessageRequestTests : SerializationTest
{
  private readonly string _testJson = @"{""model"":""claude-3-sonnet-20240229"",""system"":""test-system"",""messages"":[{""role"":""user"",""content"":[{""text"":""Hello!"",""type"":""text""}]}],""max_tokens"":512,""metadata"":{""test"":""test""},""stop_sequences"":[],""temperature"":0.5,""topK"":10,""topP"":0.5,""tool_choice"":{""type"":""auto""},""tools"":[{""name"":""test-tool"",""description"":""test-description"",""input_schema"":{""type"":""object"",""properties"":{""test-property"":{""type"":""string"",""description"":""test-description""}},""required"":[""test-property""]}}],""stream"":false}";
  [Fact]
  public void Constructor_WhenCalled_ItShouldInitializeProperties()
  {
    var model = AnthropicModels.Claude3Sonnet;
    var messages = new List<ChatMessage> { new() };
    var maxTokens = 512;
    var system = "test-system";
    var metadata = new Dictionary<string, object> { ["test"] = "test" };
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

  [Fact]
  public void Constructor_WhenCalledAndMaxTokensIsInvalid_ItShouldThrowArgumentException()
  {
    var action = () => new ChatMessageRequest(
      model: AnthropicModels.Claude3Sonnet,
      messages: [new()],
      maxTokens: 0
    );

    action.Should().Throw<ArgumentException>();
  }

  [Theory]
  [InlineData(-1)]
  [InlineData(2)]
  public void Constructor_WhenCalledAndTemperatureIsInvalid_ItShouldThrowArgumentException(decimal temperature)
  {
    var action = () => new ChatMessageRequest(
      model: AnthropicModels.Claude3Sonnet,
      messages: [new()],
      temperature: temperature
    );

    action.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void JsonSerialization_WhenSerialized_ItShouldHaveExpectedShape()
  {
    var messages = new List<ChatMessage>()
    {
      new()
      {
        Role = MessageRole.User,
        Content = [new TextContent("Hello!")]
      }
    };

    var model = AnthropicModels.Claude3Sonnet;
    var maxTokens = 512;
    var system = "test-system";
    var metadata = new Dictionary<string, object>
    {
      ["test"] = "test"
    };
    var temperature = 0.5m;
    var topK = 10;
    var topP = 0.5m;
    var toolChoice = new AutoToolChoice();
    var tools = new List<Tool>
    {
      new()
      {
        Name = "test-tool",
        Description = "test-description",
        InputSchema = new InputSchema(
          properties: new Dictionary<string, InputProperty>
          {
            ["test-property"] = new InputProperty(
              type: "string",
              description: "test-description"
            )
          },
          required: ["test-property"]
        ),
      }
    };

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

    var json = Serialize(chatMessageRequest);

    json.Should().Be(_testJson);
  }

  [Fact]
  public void JsonDeserialization_WhenDeserialized_ItShouldHaveExpectedShape()
  {
    var chatMessageRequest = Deserialize<ChatMessageRequest>(_testJson);

    chatMessageRequest!.Model.Should().Be(AnthropicModels.Claude3Sonnet);
    chatMessageRequest.System.Should().Be("test-system");
    chatMessageRequest.Messages.Should().HaveCount(1);
    chatMessageRequest.MaxTokens.Should().Be(512);
    chatMessageRequest.Metadata.Should().HaveCount(1);

    var testValue = chatMessageRequest.Metadata!.GetValueOrDefault("test")!.ToString();
    testValue.Should().Be("test");

    chatMessageRequest.Temperature.Should().Be(0.5m);
    chatMessageRequest.TopK.Should().Be(10);
    chatMessageRequest.TopP.Should().Be(0.5m);
    chatMessageRequest.ToolChoice.Should().BeOfType<AutoToolChoice>();
    chatMessageRequest.Tools.Should().HaveCount(1);
    chatMessageRequest.Tools![0].Name.Should().Be("test-tool");
    chatMessageRequest.Tools[0].Description.Should().Be("test-description");
    chatMessageRequest.Tools[0].InputSchema.Type.Should().Be("object");
    chatMessageRequest.Tools[0].InputSchema.Properties.Should().HaveCount(1);
    chatMessageRequest.Tools[0].InputSchema.Properties["test-property"].Type.Should().Be("string");
    chatMessageRequest.Tools[0].InputSchema.Properties["test-property"].Description.Should().Be("test-description");
    chatMessageRequest.Tools[0].InputSchema.Required.Should().HaveCount(1);
    chatMessageRequest.Tools[0].InputSchema.Required[0].Should().Be("test-property");
  }
}