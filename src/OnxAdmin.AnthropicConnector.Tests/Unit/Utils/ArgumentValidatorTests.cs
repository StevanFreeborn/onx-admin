namespace OnxAdmin.AnthropicConnector.Tests.Unit.Utils;

public class ArgumentValidatorTests
{
  [Fact]
  public void ThrowIfNull_WhenValueIsNull_ItShouldThrowArgumentNullException()
  {
    object? value = null;

    var action = () => ArgumentValidator.ThrowIfNull(value, "value");

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void ThrowIfNull_WhenValueIsNotNull_ItShouldNotThrow()
  {
    var value = new object();

    var action = () => ArgumentValidator.ThrowIfNull(value, "value");

    action.Should().NotThrow();
  }
}