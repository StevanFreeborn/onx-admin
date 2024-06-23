namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ContentTypeTests
{
  [Fact]
  public void Text_WhenCalled_ItShouldReturnText()
  {
    var expected = "text";

    var actual = ContentType.Text;

    actual.Should().Be(expected);
  }

  [Fact]
  public void Image_WhenCalled_ItShouldReturnImage()
  {
    var expected = "image";

    var actual = ContentType.Image;

    actual.Should().Be(expected);
  }

  [Fact]
  public void ToolUse_WhenCalled_ItShouldReturnToolUse()
  {
    var expected = "tool_use";

    var actual = ContentType.ToolUse;

    actual.Should().Be(expected);
  }

  [Fact]
  public void ToolResult_WhenCalled_ItShouldReturnToolResult()
  {
    var expected = "tool_result";

    var actual = ContentType.ToolResult;

    actual.Should().Be(expected);
  }
}