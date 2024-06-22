namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class TextContentTests : SerializationTest
{
  private readonly string _testJson = @"{""text"":""text"",""type"":""text""}";

  [Fact]
  public void Constructor_WhenCalled_ItShouldInitializeText()
  {
    var expectedText = "text";

    var result = new TextContent(expectedText);

    result.Text.Should().Be(expectedText);
  }

  [Fact]
  public void Constructor_WhenCalledAndTextIsNull_ItShouldThrowArgumentNullException()
  {
    var action = () => new TextContent(null!);

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void JsonSerialization_WhenSerialized_ItShouldHaveExpectedShape()
  {
    var content = new TextContent("text");

    var actual = Serialize(content);

    actual.Should().BeEquivalentTo(_testJson);
  }

  [Fact]
  public void JsonDeserialization_WhenDeserialized_ItShouldHaveExpectedShape()
  {
    var expected = new TextContent("text");

    var actual = Deserialize<TextContent>(_testJson);

    actual.Should().BeEquivalentTo(expected);
  }
}