namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ChatMessageRequestTests : SerializationTest
{
  private readonly string _testJson = @"{""model"":""claude-3-sonnet-20240229"",""system"":""test-system"",""messages"":[{""role"":""user"",""content"":[{""text"":""Hello!"",""type"":""text""}]}],""max_tokens"":512,""metadata"":{""test"":""test""},""stop_sequences"":[],""temperature"":0.5,""topK"":10,""topP"":0.5,""tool_choice"":{""type"":""auto""},""tools"":[{""name"":""test-tool"",""description"":""test-description"",""input_schema"":{""type"":""object"",""properties"":{""test-property"":{""type"":""string"",""description"":""test-description""}},""required"":[""test-property""]}}],""stream"":false}";
  private readonly string _testJsonWithAnyToolChoice = @"{""model"":""claude-3-sonnet-20240229"",""system"":""test-system"",""messages"":[{""role"":""user"",""content"":[{""text"":""Hello!"",""type"":""text""}]}],""max_tokens"":512,""metadata"":{""test"":""test""},""stop_sequences"":[],""temperature"":0.5,""topK"":10,""topP"":0.5,""tool_choice"":{""type"":""any""},""tools"":[{""name"":""test-tool"",""description"":""test-description"",""input_schema"":{""type"":""object"",""properties"":{""test-property"":{""type"":""string"",""description"":""test-description""}},""required"":[""test-property""]}}],""stream"":false}";
  private readonly string _testJsonWithSpecificToolChoice = @"{""model"":""claude-3-sonnet-20240229"",""system"":""test-system"",""messages"":[{""role"":""user"",""content"":[{""text"":""Hello!"",""type"":""text""}]}],""max_tokens"":512,""metadata"":{""test"":""test""},""stop_sequences"":[],""temperature"":0.5,""topK"":10,""topP"":0.5,""tool_choice"":{""type"":""tool"",""name"":""test-tool""},""tools"":[{""name"":""test-tool"",""description"":""test-description"",""input_schema"":{""type"":""object"",""properties"":{""test-property"":{""type"":""string"",""description"":""test-description""}},""required"":[""test-property""]}}],""stream"":false}";
  private readonly string _testJsonWithImageContent = @"{""model"":""claude-3-sonnet-20240229"",""system"":""test-system"",""messages"":[{""role"":""user"",""content"":[{""type"":""image"",""source"":{""media_type"": ""image/jpeg"",""data"":""data""}}]}],""max_tokens"":512,""metadata"":{""test"":""test""},""stop_sequences"":[],""temperature"":0.5,""topK"":10,""topP"":0.5,""tool_choice"":{""type"":""auto""},""tools"":[{""name"":""test-tool"",""description"":""test-description"",""input_schema"":{""type"":""object"",""properties"":{""test-property"":{""type"":""string"",""description"":""test-description""}},""required"":[""test-property""]}}],""stream"":false}";
  private readonly string _testJsonWithUnknownContent = @"{""model"":""claude-3-sonnet-20240229"",""system"":""test-system"",""messages"":[{""role"":""user"",""content"":[{""type"":""unknown"",""text"":""text""}]}],""max_tokens"":512,""metadata"":{""test"":""test""},""stop_sequences"":[],""temperature"":0.5,""topK"":10,""topP"":0.5,""tool_choice"":{""type"":""auto""},""tools"":[{""name"":""test-tool"",""description"":""test-description"",""input_schema"":{""type"":""object"",""properties"":{""test-property"":{""type"":""string"",""description"":""test-description""}},""required"":[""test-property""]}}],""stream"":false}";
  private readonly string _testJsonWithToolUseContent = @"{""model"":""claude-3-sonnet-20240229"",""system"":""test-system"",""messages"":[{""role"":""assistant"",""content"":[{""type"":""tool_use"",""name"":""test-tool"",""id"":""test-tool-id"",""input"":{""test-property"":""test-value""}}]}],""max_tokens"":512,""metadata"":{""test"":""test""},""stop_sequences"":[],""temperature"":0.5,""topK"":10,""topP"":0.5,""tool_choice"":{""type"":""auto""},""tools"":[{""name"":""test-tool"",""description"":""test-description"",""input_schema"":{""type"":""object"",""properties"":{""test-property"":{""type"":""string"",""description"":""test-description""}},""required"":[""test-property""]}}],""stream"":false}";
  private readonly string _testJsonWithToolResultContent = @"{""model"":""claude-3-sonnet-20240229"",""system"":""test-system"",""messages"":[{""role"":""assistant"",""content"":[{""type"":""tool_result"",""tool_use_id"":""test-tool"",""content"":""test-value""}]}],""max_tokens"":512,""metadata"":{""test"":""test""},""stop_sequences"":[],""temperature"":0.5,""topK"":10,""topP"":0.5,""tool_choice"":{""type"":""auto""},""tools"":[{""name"":""test-tool"",""description"":""test-description"",""input_schema"":{""type"":""object"",""properties"":{""test-property"":{""type"":""string"",""description"":""test-description""}},""required"":[""test-property""]}}],""stream"":false}";

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
    chatMessageRequest.Stream.Should().BeFalse();
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
    chatMessageRequest.ToolChoice!.Type.Should().Be("auto");
    chatMessageRequest.Tools.Should().HaveCount(1);
    chatMessageRequest.Tools![0].Name.Should().Be("test-tool");
    chatMessageRequest.Tools[0].Description.Should().Be("test-description");
    chatMessageRequest.Tools[0].InputSchema.Type.Should().Be("object");
    chatMessageRequest.Tools[0].InputSchema.Properties.Should().HaveCount(1);
    chatMessageRequest.Tools[0].InputSchema.Properties["test-property"].Type.Should().Be("string");
    chatMessageRequest.Tools[0].InputSchema.Properties["test-property"].Description.Should().Be("test-description");
    chatMessageRequest.Tools[0].InputSchema.Required.Should().HaveCount(1);
    chatMessageRequest.Tools[0].InputSchema.Required[0].Should().Be("test-property");
    chatMessageRequest.Stream.Should().BeFalse();
  }

  [Fact]
  public void JsonDeserialization_WhenDeserializedWithAnyToolChoice_ItShouldHaveExpectedShape()
  {
    var chatMessageRequest = Deserialize<ChatMessageRequest>(_testJsonWithAnyToolChoice);

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
    chatMessageRequest.ToolChoice.Should().BeOfType<AnyToolChoice>();
    chatMessageRequest.ToolChoice!.Type.Should().Be("any");
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

  [Fact]
  public void JsonDeserialization_WhenDeserializedWithSpecificToolChoice_ItShouldHaveExpectedShape()
  {
    var chatMessageRequest = Deserialize<ChatMessageRequest>(_testJsonWithSpecificToolChoice);

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
    chatMessageRequest.ToolChoice.Should().BeOfType<SpecificToolChoice>();

    var specificToolChoice = chatMessageRequest.ToolChoice as SpecificToolChoice;
    specificToolChoice!.Type.Should().Be("tool");
    specificToolChoice.Name.Should().Be("test-tool");
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

  [Fact]
  public void JsonDeserialization_WhenDeserializedWithUnknownToolChoice_ItShouldThrowJsonException()
  {
    var json = @"{""model"":""claude-3-sonnet-20240229"",""system"":""test-system"",""messages"":[{""role"":""user"",""content"":[{""text"":""Hello!"",""type"":""text""}]}],""max_tokens"":512,""metadata"":{""test"":""test""},""stop_sequences"":[],""temperature"":0.5,""topK"":10,""topP"":0.5,""tool_choice"":{""type"":""unknown""},""tools"":[{""name"":""test-tool"",""description"":""test-description"",""input_schema"":{""type"":""object"",""properties"":{""test-property"":{""type"":""string"",""description"":""test-description""}},""required"":[""test-property""]}}],""stream"":false}";

    var action = () => Deserialize<ChatMessageRequest>(json);

    action.Should().Throw<JsonException>();
  }

  [Fact]
  public void JsonDeserialization_WhenDeserializedWithImageContent_ItShouldHaveExpectedShape()
  {
    var chatMessageRequest = Deserialize<ChatMessageRequest>(_testJsonWithImageContent);

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
    chatMessageRequest.ToolChoice!.Type.Should().Be("auto");
    chatMessageRequest.Tools.Should().HaveCount(1);
    chatMessageRequest.Tools![0].Name.Should().Be("test-tool");
    chatMessageRequest.Tools[0].Description.Should().Be("test-description");
    chatMessageRequest.Tools[0].InputSchema.Type.Should().Be("object");
    chatMessageRequest.Tools[0].InputSchema.Properties.Should().HaveCount(1);
    chatMessageRequest.Tools[0].InputSchema.Properties["test-property"].Type.Should().Be("string");
    chatMessageRequest.Tools[0].InputSchema.Properties["test-property"].Description.Should().Be("test-description");
    chatMessageRequest.Tools[0].InputSchema.Required.Should().HaveCount(1);
    chatMessageRequest.Tools[0].InputSchema.Required[0].Should().Be("test-property");

    chatMessageRequest.Messages[0].Content.Should().HaveCount(1);
    chatMessageRequest.Messages[0].Content[0].Should().BeOfType<ImageContent>();

    var imageContent = chatMessageRequest.Messages[0].Content[0] as ImageContent;
    imageContent!.Type.Should().Be("image");
    imageContent.Source.MediaType.Should().Be("image/jpeg");
    imageContent.Source.Data.Should().Be("data");
  }

  [Fact]
  public void JsonDeserialization_WhenDeserializedWithToolUseContent_ItShouldHaveExpectedShape()
  {
    var chatMessageRequest = Deserialize<ChatMessageRequest>(_testJsonWithToolUseContent);

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
    chatMessageRequest.ToolChoice!.Type.Should().Be("auto");
    chatMessageRequest.Tools.Should().HaveCount(1);
    chatMessageRequest.Tools![0].Name.Should().Be("test-tool");
    chatMessageRequest.Tools[0].Description.Should().Be("test-description");
    chatMessageRequest.Tools[0].InputSchema.Type.Should().Be("object");
    chatMessageRequest.Tools[0].InputSchema.Properties.Should().HaveCount(1);
    chatMessageRequest.Tools[0].InputSchema.Properties["test-property"].Type.Should().Be("string");
    chatMessageRequest.Tools[0].InputSchema.Properties["test-property"].Description.Should().Be("test-description");
    chatMessageRequest.Tools[0].InputSchema.Required.Should().HaveCount(1);
    chatMessageRequest.Tools[0].InputSchema.Required[0].Should().Be("test-property");

    chatMessageRequest.Messages[0].Content.Should().HaveCount(1);
    chatMessageRequest.Messages[0].Content[0].Should().BeOfType<ToolUseContent>();

    var toolUseContent = chatMessageRequest.Messages[0].Content[0] as ToolUseContent;
    toolUseContent!.Type.Should().Be("tool_use");
    toolUseContent.Name.Should().Be("test-tool");
    toolUseContent.Id.Should().Be("test-tool-id");
    toolUseContent.Input.Should().HaveCount(1);
    toolUseContent.Input["test-property"].ToString().Should().Be("test-value");
  }

  [Fact]
  public void JsonDeserialization_WhenDeserializedWithToolResultContent_ItShouldHaveExpectedShape()
  {
    var chatMessageRequest = Deserialize<ChatMessageRequest>(_testJsonWithToolResultContent);

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
    chatMessageRequest.ToolChoice!.Type.Should().Be("auto");
    chatMessageRequest.Tools.Should().HaveCount(1);
    chatMessageRequest.Tools![0].Name.Should().Be("test-tool");
    chatMessageRequest.Tools[0].Description.Should().Be("test-description");
    chatMessageRequest.Tools[0].InputSchema.Type.Should().Be("object");
    chatMessageRequest.Tools[0].InputSchema.Properties.Should().HaveCount(1);
    chatMessageRequest.Tools[0].InputSchema.Properties["test-property"].Type.Should().Be("string");
    chatMessageRequest.Tools[0].InputSchema.Properties["test-property"].Description.Should().Be("test-description");
    chatMessageRequest.Tools[0].InputSchema.Required.Should().HaveCount(1);
    chatMessageRequest.Tools[0].InputSchema.Required[0].Should().Be("test-property");

    chatMessageRequest.Messages[0].Content.Should().HaveCount(1);
    chatMessageRequest.Messages[0].Content[0].Should().BeOfType<ToolResultContent>();

    var toolResultContent = chatMessageRequest.Messages[0].Content[0] as ToolResultContent;
    toolResultContent!.Type.Should().Be("tool_result");
    toolResultContent.ToolUseId.Should().Be("test-tool");
    toolResultContent.Content.Should().Be("test-value");
  }

  [Fact]
  public void JsonDeserialization_WhenDeserializedWithUnknownContent_ItShouldThrowJsonException()
  {
    var action = () => Deserialize<ChatMessageRequest>(_testJsonWithUnknownContent);

    action.Should().Throw<JsonException>();
  }
}