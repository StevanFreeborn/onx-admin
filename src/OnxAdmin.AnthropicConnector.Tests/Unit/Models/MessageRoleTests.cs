namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class MessageRoleTests
{
  [Fact]
  public void User_WhenCalled_ItShouldReturnUser()
  {
    var expected = "user";

    var actual = MessageRole.User;

    actual.Should().Be(expected);
  }

  [Fact]
  public void Assistant_WhenCalled_ItShouldReturnAssistant()
  {
    var expected = "assistant";

    var actual = MessageRole.Assistant;

    actual.Should().Be(expected);
  }

  [Theory]
  [InlineData("user", true)]
  [InlineData("assistant", true)]
  [InlineData("invalid", false)]
  public void IsValidRole_WhenCalled_ItShouldReturnExpectedValue(string role, bool expected)
  {
    var actual = MessageRole.IsValidRole(role);

    actual.Should().Be(expected);
  }
}