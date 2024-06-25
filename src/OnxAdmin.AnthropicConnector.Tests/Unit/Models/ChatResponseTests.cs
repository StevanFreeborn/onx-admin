namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ChatResponseTests : SerializationTest
{
  [Fact]
  public void Constructor_WhenCalled_ShouldInitializeProperties()
  {
    var expectedId = "id";
    var expectedModel = "model";
    var expectedRole = "role";
    var expectedStopReason = "stop reason";
    var expectedStopSequence = "stop sequence";
    var expectedType = "type";
    var expectedUsage = new ChatUsage
    {
      InputTokens = 1,
      OutputTokens = 2
    };
    var expectedContent = new List<Content>
    {
      new TextContent("text content"),
    };

    var chatResponse = new ChatResponse
    {
      Id = expectedId,
      Model = expectedModel,
      Role = expectedRole,
      StopReason = expectedStopReason,
      StopSequence = expectedStopSequence,
      Type = expectedType,
      Usage = expectedUsage,
      Content = expectedContent
    };

    chatResponse.Id.Should().Be(expectedId);
    chatResponse.Model.Should().Be(expectedModel);
    chatResponse.Role.Should().Be(expectedRole);
    chatResponse.StopReason.Should().Be(expectedStopReason);
    chatResponse.StopSequence.Should().Be(expectedStopSequence);
    chatResponse.Type.Should().Be(expectedType);
    chatResponse.Usage.Should().BeEquivalentTo(expectedUsage);
    chatResponse.Content.Should().BeEquivalentTo(expectedContent);
  }

  [Fact]
  public void JsonSerialization_WhenCalled_ItShouldSerializeCorrectly()
  {
    var expectedJson = @"{""id"":""id"",""model"":""model"",""role"":""role"",""stop_reason"":""stop reason"",""stop_sequence"":""stop sequence"",""type"":""type"",""usage"":{""input_tokens"":1,""output_tokens"":2},""content"":[{""text"":""text content"",""type"":""text""}]}";

    var chatResponse = new ChatResponse
    {
      Id = "id",
      Model = "model",
      Role = "role",
      StopReason = "stop reason",
      StopSequence = "stop sequence",
      Type = "type",
      Usage = new ChatUsage
      {
        InputTokens = 1,
        OutputTokens = 2
      },
      Content =
      [
        new TextContent("text content"),
      ]
    };

    var json = Serialize(chatResponse);

    json.Should().Be(expectedJson);
  }

  [Fact]
  public void JsonDeserialization_WhenCalled_ItShouldDeserializeCorrectly()
  {
    var json = @"{""id"":""id"",""model"":""model"",""role"":""role"",""stop_reason"":""stop reason"",""stop_sequence"":""stop sequence"",""type"":""type"",""usage"":{""input_tokens"":1,""output_tokens"":2},""content"":[{""text"":""text content"",""type"":""text""}]}";

    var chatResponse = Deserialize<ChatResponse>(json);

    chatResponse!.Id.Should().Be("id");
    chatResponse.Model.Should().Be("model");
    chatResponse.Role.Should().Be("role");
    chatResponse.StopReason.Should().Be("stop reason");
    chatResponse.StopSequence.Should().Be("stop sequence");
    chatResponse.Type.Should().Be("type");
    chatResponse.Usage.Should().BeEquivalentTo(new ChatUsage
    {
      InputTokens = 1,
      OutputTokens = 2
    });
    chatResponse.Content.Should().BeEquivalentTo(new List<Content>
    {
      new TextContent("text content"),
    });
  }
}