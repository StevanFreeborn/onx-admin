namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class InputPropertyTests : SerializationTest
{
  private readonly string _testJson = @"{""type"":""type"",""description"":""description""}";

  [Fact]
  public void Constructor_WhenCalled_ShouldInitializeProperties()
  {
    var type = "type";
    var description = "description";

    var inputProperty = new InputProperty(type, description);

    inputProperty.Type.Should().Be(type);
    inputProperty.Description.Should().Be(description);
  }

  [Fact]
  public void Constructor_WhenTypeIsNull_ShouldThrowArgumentNullException()
  {
    var description = "description";

    var action = () => new InputProperty(null!, description);

    action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'type')");
  }

  [Fact]
  public void Constructor_WhenDescriptionIsNull_ShouldThrowArgumentNullException()
  {
    var type = "type";

    var action = () => new InputProperty(type, null!);

    action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'description')");
  }

  [Fact]
  public void JsonSerialization_WhenSerialized_ItShouldHaveExpectedShape()
  {
    var inputProperty = new InputProperty("type", "description");

    var json = Serialize(inputProperty);

    json.Should().Be(_testJson);
  }

  [Fact]
  public void JsonDeserialization_WhenDeserialized_ItShouldHaveExpectedShape()
  {
    var inputProperty = Deserialize<InputProperty>(_testJson);

    inputProperty!.Type.Should().Be("type");
    inputProperty.Description.Should().Be("description");
  }
}