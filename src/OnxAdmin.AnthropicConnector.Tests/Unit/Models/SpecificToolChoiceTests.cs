namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class SpecificToolChoiceTests
{
  [Fact]
  public void Constructor_WhenCalled_ItShouldSetTypeToTool()
  {
    var expectedName = "tool";
    var expectedType = "tool";

    var actual = new SpecificToolChoice("tool");

    actual.Name.Should().Be(expectedName);
    actual.Type.Should().Be(expectedType);
  }

  [Fact]
  public void Constructor_WhenCalledWithNull_ItShouldThrowArgumentNullException()
  {
    Action action = () => new SpecificToolChoice(null!);

    action.Should().Throw<ArgumentNullException>();
  }
}