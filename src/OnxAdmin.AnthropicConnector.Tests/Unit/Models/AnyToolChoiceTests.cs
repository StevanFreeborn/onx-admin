namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class AnyToolChoiceTests
{
  [Fact]
  public void Constructor_WhenCalled_ItShouldSetTypeToAny()
  {
    var expected = "any";

    var actual = new AnyToolChoice();

    actual.Type.Should().Be(expected);
  }
}