namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ToolChoiceTypeTests
{
  [Fact]
  public void Auto_WhenCalled_ItShouldReturnAuto()
  {
    var expected = "auto";

    var actual = ToolChoiceType.Auto;

    actual.Should().Be(expected);
  }

  [Fact]
  public void Any_WhenCalled_ItShouldReturnAny()
  {
    var expected = "any";

    var actual = ToolChoiceType.Any;

    Assert.Equal(expected, actual);
  }

  [Fact]
  public void Tool_WhenCalled_ItShouldReturnTool()
  {
    var expected = "tool";

    var actual = ToolChoiceType.Tool;

    Assert.Equal(expected, actual);
  }

  [Theory]
  [InlineData("auto", true)]
  [InlineData("any", true)]
  [InlineData("tool", true)]
  [InlineData("invalid", false)]
  public void IsValidType_WhenCalledWithType_ItShouldReturnExpectedResult(string type, bool expected)
  {
    var actual = ToolChoiceType.IsValidType(type);

    Assert.Equal(expected, actual);
  }
}