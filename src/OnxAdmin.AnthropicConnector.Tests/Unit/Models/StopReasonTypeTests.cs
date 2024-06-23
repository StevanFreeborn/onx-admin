namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class StopReasonTypeTests
{
  [Fact]
  public void EndTurn_WhenCalled_ItShouldReturnEndTurn()
  {
    var expected = "end_turn";

    var actual = StopReasonType.EndTurn;

    actual.Should().Be(expected);
  }

  [Fact]
  public void MaxTokens_WhenCalled_ItShouldReturnMaxTokens()
  {
    var expected = "max_tokens";

    var actual = StopReasonType.MaxTokens;

    actual.Should().Be(expected);
  }

  [Fact]
  public void StopSequence_WhenCalled_ItShouldReturnStopSequence()
  {
    var expected = "stop_sequence";

    var actual = StopReasonType.StopSequence;

    actual.Should().Be(expected);
  }

  [Fact]
  public void ToolUse_WhenCalled_ItShouldReturnToolUse()
  {
    var expected = "tool_use";

    var actual = StopReasonType.ToolUse;

    actual.Should().Be(expected);
  }
}