namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class SpecificToolChoiceTests : SerializationTest
{
  private readonly string _testToolName = "tool";
  private string GetTestJson(string name) => @$"{{""name"":""{name}"",""type"":""tool""}}";

  [Fact]
  public void Constructor_WhenCalled_ItShouldSetTypeToTool()
  {
    var expectedType = "tool";

    var actual = new SpecificToolChoice(_testToolName);

    actual.Name.Should().Be(_testToolName);
    actual.Type.Should().Be(expectedType);
  }

  [Fact]
  public void Constructor_WhenCalledWithNull_ItShouldThrowArgumentNullException()
  {
    Action action = () => new SpecificToolChoice(null!);

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void JsonSerialization_WhenSerialized_ItShouldHaveExpectedShape()
  {
    var choice = new SpecificToolChoice(_testToolName);

    var actual = Serialize(choice);

    actual.Should().BeEquivalentTo(GetTestJson(_testToolName));
  }

  [Fact]
  public void JsonDeserialization_WhenDeserialized_ItShouldHaveExpectedShape()
  {
    var expected = new SpecificToolChoice(_testToolName);

    var actual = Deserialize<SpecificToolChoice>(GetTestJson(_testToolName));

    actual.Should().BeEquivalentTo(expected);
  }
}