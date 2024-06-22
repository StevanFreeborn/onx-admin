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

  [Theory]
  [InlineData("text", true)]
  [InlineData("image", true)]
  [InlineData("invalid", false)]
  public void IsValidContentType_WhenCalled_ItShouldReturnExpectedValue(string contentType, bool expected)
  {
    var actual = ContentType.IsValidContentType(contentType);

    actual.Should().Be(expected);
  }
}