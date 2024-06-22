namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class AnthropicModelsTests
{
  [Fact]
  public void Claude3Opus_WhenCalled_ItShouldReturnExpectedValue()
  {
    var expected = "claude-3-opus-20240229";

    var actual = AnthropicModels.Claude3Opus;

    actual.Should().Be(expected);
  }

  [Fact]
  public void Claude3Sonnet_WhenCalled_ItShouldReturnExpectedValue()
  {
    var expected = "claude-3-sonnet-20240229";

    var actual = AnthropicModels.Claude3Sonnet;

    actual.Should().Be(expected);
  }

  [Fact]
  public void Claude35Sonnet_WhenCalled_ItShouldReturnExpectedValue()
  {
    var expected = "claude-3-5-sonnet-20240620";

    var actual = AnthropicModels.Claude35Sonnet;

    actual.Should().Be(expected);
  }

  [Fact]
  public void Claude3Haiku_WhenCalled_ItShouldReturnExpectedValue()
  {
    var expected = "claude-3-haiku-20240307";

    var actual = AnthropicModels.Claude3Haiku;

    actual.Should().Be(expected);
  }

  [Fact]
  public void Claude35Sonnet_WhenCalled_ItShouldExpectedValue()
  {
    var expected = "claude-3-5-sonnet-20240620";

    var actual = AnthropicModels.Claude35Sonnet;

    actual.Should().Be(expected);
  }

  [Theory]
  [InlineData("claude-3-opus-20240229", true)]
  [InlineData("claude-3-sonnet-20240229", true)]
  [InlineData("claude-3-5-sonnet-20240620", true)]
  [InlineData("claude-3-haiku-20240307", true)]
  [InlineData("invalid", false)]
  public void IsValidModel_WhenCalled_ItShouldReturnExpectedValue(string modelId, bool expected)
  {
    var actual = AnthropicModels.IsValidModel(modelId);

    actual.Should().Be(expected);
  }
}