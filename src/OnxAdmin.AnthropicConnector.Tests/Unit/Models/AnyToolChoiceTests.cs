namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class AnyToolChoiceTests : SerializationTest
{
  private readonly string _testJson = @"{""type"":""any""}";

  [Fact]
  public void Constructor_WhenCalled_ItShouldSetTypeToAny()
  {
    var expected = "any";

    var actual = new AnyToolChoice();

    actual.Type.Should().Be(expected);
  }

  [Fact]
  public void JsonSerialization_WhenSerialized_ItShouldHaveExpectedShape()
  {
    var choice = new AnyToolChoice();

    var actual = Serialize(choice);

    actual.Should().BeEquivalentTo(_testJson);
  }

  [Fact]
  public void JsonDeserialization_WhenDeserialized_ItShouldHaveExpectedShape()
  {
    var expected = new AnyToolChoice();

    var actual = Deserialize<AnyToolChoice>(_testJson);

    actual.Should().BeEquivalentTo(expected);
  }
}