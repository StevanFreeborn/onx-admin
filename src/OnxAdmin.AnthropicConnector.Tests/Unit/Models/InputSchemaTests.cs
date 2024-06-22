namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class InputSchemaTests : SerializationTest
{
  private readonly string _testJson = @$"{{""type"":""object"",""properties"":{{""property1"":{{""type"":""type1"",""description"":""description1""}},""property2"":{{""type"":""type2"",""description"":""description2""}}}},""required"":[""property1""]}}";

  [Fact]
  public void Constructor_WhenCalled_ItShouldInitializeProperties()
  {
    var inputProperties = new Dictionary<string, InputProperty>
    {
      { "property1", new InputProperty("type1", "description1") },
      { "property2", new InputProperty("type2", "description2") }
    };

    var required = new List<string> { "property1" };

    var inputSchema = new InputSchema(
      properties: inputProperties,
      required: required
    );

    inputSchema.Type.Should().Be("object");
    inputSchema.Properties.Should().BeSameAs(inputProperties);
    inputSchema.Required.Should().BeSameAs(required);
  }

  [Fact]
  public void Constructor_WhenPropertiesIsNull_ItShouldThrowArgumentNullException()
  {
    var required = new List<string> { "property1" };

    var action = () => new InputSchema(null!, required);

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WhenRequiredIsNull_ItShouldThrowArgumentNullException()
  {
    var inputProperties = new Dictionary<string, InputProperty>
    {
      { "property1", new InputProperty("type1", "description1") },
      { "property2", new InputProperty("type2", "description2") }
    };

    var action = () => new InputSchema(inputProperties, null!);

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WhenRequiredPropertyIsNotInProperties_ItShouldThrowArgumentException()
  {
    var inputProperties = new Dictionary<string, InputProperty>
    {
      { "property1", new InputProperty("type1", "description1") },
      { "property2", new InputProperty("type2", "description2") }
    };

    var required = new List<string> { "property3" };

    var action = () => new InputSchema(inputProperties, required);

    action.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void JsonSerialization_WhenSerialized_ItShouldHaveExpectedShape()
  {
    var inputProperties = new Dictionary<string, InputProperty>
    {
      { "property1", new InputProperty("type1", "description1") },
      { "property2", new InputProperty("type2", "description2") }
    };

    var required = new List<string> { "property1" };

    var inputSchema = new InputSchema(
      properties: inputProperties,
      required: required
    );

    var actual = Serialize(inputSchema);

    actual.Should().Be(_testJson);
  }

  [Fact]
  public void JsonDeserialization_WhenDeserialized_ItShouldHaveExpectedShape()
  {
    var inputProperties = new Dictionary<string, InputProperty>
    {
      { "property1", new InputProperty("type1", "description1") },
      { "property2", new InputProperty("type2", "description2") }
    };

    var required = new List<string> { "property1" };

    var expected = new InputSchema(
      properties: inputProperties,
      required: required
    );

    var actual = Deserialize<InputSchema>(_testJson);

    actual.Should().BeEquivalentTo(expected);
  }
}