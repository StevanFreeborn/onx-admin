namespace OnxAdmin.AnthropicConnector.Tests.Unit.Models;

public class ToolTests : SerializationTest
{
  private readonly string _testJson = @"{""name"":""test-name"",""description"":""test-description"",""input_schema"":{""type"":""object"",""properties"":{},""required"":[]}}";

  [Fact]
  public void Constructor_WhenCalled_ItShouldInitializeProperties()
  {
    var name = "test-name";
    var description = "test-description";
    var inputSchema = new InputSchema();

    var tool = new Tool(
      name: name,
      description: description,
      inputSchema: inputSchema
    );

    tool.Name.Should().Be(name);
    tool.Description.Should().Be(description);
    tool.InputSchema.Should().Be(inputSchema);
  }

  [Fact]
  public void Constructor_WhenCalledAndNameIsNull_ItShouldThrowArgumentNullException()
  {
    var action = () => new Tool(
      name: null!,
      description: "test-description",
      inputSchema: new()
    );

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WhenCalledAndDescriptionIsNull_ItShouldThrowArgumentNullException()
  {
    var action = () => new Tool(
      name: "test-name",
      description: null!,
      inputSchema: new()
    );

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_WhenCalledAndInputSchemaIsNull_ItShouldThrowArgumentNullException()
  {
    var action = () => new Tool(
      name: "test-name",
      description: "test-description",
      inputSchema: null!
    );

    action.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void JsonSerialization_WhenSerialized_ItShouldHaveExpectedShape()
  {
    var tool = new Tool(
      name: "test-name",
      description: "test-description",
      inputSchema: new()
    );

    var json = Serialize(tool);

    json.Should().Be(_testJson);
  }

  [Fact]
  public void JsonDeserialization_WhenDeserialized_ItShouldHaveExpectedShape()
  {
    var tool = Deserialize<Tool>(_testJson);

    tool!.Name.Should().Be("test-name");
    tool.Description.Should().Be("test-description");
    tool.InputSchema.Should().BeEquivalentTo(new InputSchema());
  }
}