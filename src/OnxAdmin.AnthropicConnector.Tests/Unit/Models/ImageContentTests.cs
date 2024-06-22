namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ImageContentTests : SerializationTest
{
  private readonly string _testJson = @"{""source"":{""mediaType"":""image/png"",""data"":""data""},""type"":""image""}";

  [Fact]
  public void Constructor_WhenCalled_ItShouldInitializeSource()
  {
    var expectedMediaType = "image/png";
    var expectedData = "data";

    var result = new ImageContent(expectedMediaType, expectedData);

    result.Source.Should().BeEquivalentTo(new ImageSource(expectedMediaType, expectedData));
  }

  [Fact]
  public void Constructor_WhenCalledAndMediaTypeIsNull_ItShouldThrowArgumentNullException()
  {
    var expectedData = "data";

    var action = () => new ImageContent(null!, expectedData);

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WhenCalledAndDataIsNull_ItShouldThrowArgumentNullException()
  {
    var expectedMediaType = "image/png";

    var action = () => new ImageContent(expectedMediaType, null!);

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WhenCalledWithInvalidMediaType_ItShouldThrowArgumentException()
  {
    var expectedMediaType = "invalid";
    var expectedData = "data";

    var action = () => new ImageContent(expectedMediaType, expectedData);

    action.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void JsonSerialization_WhenSerialized_ItShouldHaveExpectedShape()
  {
    var content = new ImageContent("image/png", "data");

    var actual = Serialize(content);

    actual.Should().BeEquivalentTo(_testJson);
  }

  [Fact]
  public void JsonDeserialization_WhenDeserialized_ItShouldHaveExpectedShape()
  {
    var expected = new ImageContent("image/png", "data");

    var actual = Deserialize<ImageContent>(_testJson);

    actual.Should().BeEquivalentTo(expected);
  }
}