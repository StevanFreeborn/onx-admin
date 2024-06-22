namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ImageTypeTests
{
  [Fact]
  public void Jpg_WhenCalled_ItShouldReturnJpg()
  {
    var expected = "image/jpeg";

    var actual = ImageType.Jpg;

    actual.Should().Be(expected);
  }

  [Fact]
  public void Png_WhenCalled_ItShouldReturnPng()
  {
    var expected = "image/png";

    var actual = ImageType.Png;

    actual.Should().Be(expected);
  }

  [Fact]
  public void Gif_WhenCalled_ItShouldReturnGif()
  {
    var expected = "image/gif";

    var actual = ImageType.Gif;

    actual.Should().Be(expected);
  }

  [Fact]
  public void Webp_WhenCalled_ItShouldReturnWebp()
  {
    var expected = "image/webp";

    var actual = ImageType.Webp;

    actual.Should().Be(expected);
  }

  [Theory]
  [InlineData("image/jpeg", true)]
  [InlineData("image/png", true)]
  [InlineData("image/gif", true)]
  [InlineData("image/webp", true)]
  [InlineData("invalid", false)]
  public void IsValidImageType_WhenCalled_ItShouldReturnExpectedValue(string imageType, bool expected)
  {
    var actual = ImageType.IsValidImageType(imageType);

    actual.Should().Be(expected);
  }
}