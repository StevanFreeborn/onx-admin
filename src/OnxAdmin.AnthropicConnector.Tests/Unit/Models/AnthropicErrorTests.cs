namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class AnthropicErrorTests : SerializationTest
{
  [Fact]
  public void Constructor_WhenCalled_ItShouldInitializeProperties()
  {
    var apiError = new ApiError("message");

    var error = new AnthropicError(apiError);

    error.Type.Should().Be("error");
    error.Error.Should().BeSameAs(apiError);
  }

  [Fact]
  public void JsonSerialization_WhenSerialized_ItShouldBeExpectedShape()
  {
    var expectedJson = @"{""type"":""error"",""error"":{""type"":""api_error"",""message"":""message""}}";

    var apiError = new ApiError("message");
    var error = new AnthropicError(apiError);

    var json = Serialize(error);

    json.Should().Be(expectedJson);
  }

  [Fact]
  public void JsonDeserialization_WhenDeserialized_ItShouldBeExpectedModel()
  {
    var json = @"{""type"":""error"",""error"":{""message"":""message"",""type"":""api_error""}}";

    var error = Deserialize<AnthropicError>(json);

    error!.Type.Should().Be("error");
    error.Error.Should().BeOfType<ApiError>();
    error.Error!.Message.Should().Be("message");
  }
}