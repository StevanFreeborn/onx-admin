namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ChatUsageTests : SerializationTest
{
  [Fact]
  public void Constructor_WhenCalled_ShouldInitializeProperties()
  {
    var expectedInputTokens = 1;
    var expectedOutputTokens = 2;

    var chatUsage = new ChatUsage
    {
      InputTokens = expectedInputTokens,
      OutputTokens = expectedOutputTokens
    };

    chatUsage.InputTokens.Should().Be(expectedInputTokens);
    chatUsage.OutputTokens.Should().Be(expectedOutputTokens);
  }

  [Fact]
  public void JsonSerialization_WhenCalled_ItShouldSerializeCorrectly()
  {
    var expectedJson = @"{""input_tokens"":1,""output_tokens"":2}";

    var chatUsage = new ChatUsage
    {
      InputTokens = 1,
      OutputTokens = 2
    };

    var json = Serialize(chatUsage);

    json.Should().Be(expectedJson);
  }

  [Fact]
  public void JsonDeserialization_WhenCalled_ItShouldDeserializeCorrectly()
  {
    var json = @"{""input_tokens"":1,""output_tokens"":2}";

    var chatUsage = Deserialize<ChatUsage>(json);

    chatUsage!.InputTokens.Should().Be(1);
    chatUsage.OutputTokens.Should().Be(2);
  }
}