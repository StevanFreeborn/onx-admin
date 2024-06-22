namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class AutoToolChoiceTests
{
  [Fact]
  public void Constructor_WhenCalled_ItShouldSetTypeToAuto()
  {
    var expected = "auto";

    var actual = new AutoToolChoice();

    actual.Type.Should().Be(expected);
  }
}