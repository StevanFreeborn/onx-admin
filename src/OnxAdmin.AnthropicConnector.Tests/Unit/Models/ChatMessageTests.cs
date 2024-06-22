namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ChatMessageTests : SerializationTest
{
  private readonly string _testJson = @"{""role"":""assistant"",""content"":[{""text"":""text"",""type"":""text""}]}";

  [Fact]
  public void Constructor_WhenCalled_ItShouldInitializeProperties()
  {
    var role = "assistant";
    var content = new List<Content> { new TextContent("text") };

    var chatMessage = new ChatMessage(role, content);

    chatMessage.Role.Should().Be(role);
    chatMessage.Content.Should().BeSameAs(content);
  }

  [Fact]
  public void Constructor_WhenCalledAndRoleIsNull_ItShouldThrowArgumentNullException()
  {
    var content = new List<Content> { new TextContent("text") };

    var action = () => new ChatMessage(null!, content);

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WhenCalledAndContentIsNull_ItShouldThrowArgumentNullException()
  {
    var role = "assistant";

    var action = () => new ChatMessage(role, null!);

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WhenCalledAndRoleIsInvalid_ItShouldThrowArgumentException()
  {
    var role = "invalid";
    var content = new List<Content> { new TextContent("text") };

    var action = () => new ChatMessage(role, content);

    action.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void JsonSerialization_WhenSerialized_ItShouldReturnJsonString()
  {
    var role = "assistant";
    var content = new List<Content> { new TextContent("text") };
    var chatMessage = new ChatMessage(role, content);

    var json = Serialize(chatMessage);

    json.Should().Be(_testJson);
  }

  [Fact]
  public void JsonDeserialization_WhenDeserialized_ItShouldReturnChatMessage()
  {
    var chatMessage = Deserialize<ChatMessage>(_testJson);

    chatMessage.Should().NotBeNull();
    chatMessage!.Role.Should().Be("assistant");
    chatMessage.Content.Should().HaveCount(1);
    chatMessage.Content[0].Should().BeOfType<TextContent>();
    chatMessage.Content[0].As<TextContent>().Text.Should().Be("text");
  }
}