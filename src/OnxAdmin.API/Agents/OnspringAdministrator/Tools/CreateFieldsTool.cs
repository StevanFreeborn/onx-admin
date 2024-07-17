
namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class CreateFieldsTool(
  IOnspringAdmin admin
) : IOnspringAdministratorTool
{
  private const string Name = "Create_Fields";
  private const string Description = """
    This tool can be used to create fields in an Onspring app. 
    It will create the fields provided.
    It will return a message indicating which fields were created successfully and which fields failed to be created.
    The message will also include a link to the app where the fields were created.
  """;
  private readonly IOnspringAdmin _admin = admin;

  public async Task<string> CreateFields(
    [FunctionParameter("The name of the app where the field should be created create", required: true)]
    string appName,
    [FunctionParameter("The fields to create. Each field should have a name, type, and description.", required: true)]
    List<Field> fields
  )
  {
    return await _admin.PerformActionAsync(async page =>
    {
      await page.GotoAsync($"/Admin/App");
      await page.GetByPlaceholder(new Regex("filter by", RegexOptions.IgnoreCase)).PressSequentiallyAsync(appName, new() { Delay = 150 });
      await page.WaitForResponseAsync(new Regex("/Admin/App/AppsListRead", RegexOptions.IgnoreCase));

      var appRow = page.GetByRole(AriaRole.Row, new() { NameRegex = new Regex(appName, RegexOptions.IgnoreCase), Exact = true }).First;
      var isRowVisible = await appRow.IsVisibleAsync();

      if (isRowVisible is false)
      {
        return $"Unable to create fields because the app {appName} was not found.";
      }

      await appRow.ClickAsync();
      await page.WaitForURLAsync(new Regex(@"/Admin/App/\d+", RegexOptions.IgnoreCase));
      await page.GetByRole(AriaRole.Tab, new() { NameRegex = new("layouts", RegexOptions.IgnoreCase) }).ClickAsync();

      var msg = new StringBuilder();

      foreach (var field in fields)
      {
        var resultMsg = await CreateField(page, field.Type, field.Name, field.Description);
        msg.AppendLine(resultMsg);
      }

      var layoutResult = await AddFieldsToLayout(page, "default", fields);
      msg.AppendLine(layoutResult);

      msg.AppendLine($"The app where the fields were created can be found at {page.Url}");

      return msg.ToString();
    });
  }

  private async Task<string> AddFieldsToLayout(IPage page, string layoutName, List<Field> fields)
  {
    await page.Locator("#grid-layouts").GetByRole(AriaRole.Row, new() { NameRegex = new(layoutName, RegexOptions.IgnoreCase) }).ClickAsync();
    var layoutDesigner = page.GetByRole(AriaRole.Dialog);
    await layoutDesigner.WaitForAsync();

    foreach (var field in fields)
    {
      var frame = layoutDesigner.FrameLocator("iframe");
      var fieldToDrag = frame.Locator(".item-lists").Locator($".layoutItem.draggable:has-text('{field.Name}')").First;
      var dropZone = frame.Locator(".mainContainer").Locator("[data-column='0'][data-row]").Last;
      var dropLocation = frame.Locator("#dropLocation");

      await fieldToDrag.HoverAsync();
      await page.Mouse.DownAsync();
      await dropZone.HoverAsync();
      await dropLocation.WaitForAsync();
      await page.Mouse.UpAsync();
    }

    await layoutDesigner.GetByRole(AriaRole.Button, new() { NameRegex = new("save & close", RegexOptions.IgnoreCase) }).ClickAsync();
    await layoutDesigner.WaitForAsync(new() { State = WaitForSelectorState.Hidden });

    return "Fields added to layout named {layoutName} successfully.";
  }

  private async Task<string> CreateField(IPage page, FieldType type, string name, string description)
  {
    await page.Locator("[data-add-button='layout-item']").ClickAsync();

    var layoutItemMenu = page.Locator("[data-add-menu='layout-item']");
    await layoutItemMenu.WaitForAsync();
    await layoutItemMenu.GetByText(new Regex($"{type}", RegexOptions.IgnoreCase)).First.ClickAsync();

    var addFieldDialog = page.GetByRole(AriaRole.Dialog, new() { NameRegex = new($"add.*field", RegexOptions.IgnoreCase) });
    await addFieldDialog.WaitForAsync();
    await addFieldDialog.GetByRole(AriaRole.Button, new() { NameRegex = new("continue", RegexOptions.IgnoreCase) }).ClickAsync();
    await addFieldDialog.WaitForAsync();

    var frame = addFieldDialog.FrameLocator("iframe");
    await frame.Locator(".label:has-text('Field') + .data input").FillAsync(name);
    await frame.Locator(".label:has-text('Description') + .data .content-area.mce-content-body").FillAsync(description);
    await addFieldDialog.GetByRole(AriaRole.Button, new() { NameRegex = new("save", RegexOptions.IgnoreCase) }).ClickAsync();

    var addFieldResponse = await page.WaitForResponseAsync(new Regex(@"/Admin/App/\d+/Field/AddUsingSettings", RegexOptions.IgnoreCase));

    if (addFieldResponse.Ok is false)
    {
      return $"Unable to create {name} field.";
    }

    var addFieldResponseJson = await addFieldResponse.JsonAsync();

    if (addFieldResponseJson.Value.TryGetProperty("success", out var success) && success.GetBoolean() is false)
    {
      var errors = addFieldResponseJson.Value.GetProperty("errors").EnumerateArray().Select(e => e.GetProperty("message").GetString()).ToList();
      var message = string.Join(Environment.NewLine, errors);
      return $"Unable to create {name} field due to the following errors:{Environment.NewLine}{message}";
    }

    return $"{name} field created successfully.";
  }

  public Tool Create()
  {
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(CreateFields));
  }
}

record Field
{
  [FunctionProperty("The name of the field.", required: true)]
  public string Name { get; init; } = string.Empty;

  [FunctionProperty("The type of the field.", required: true)]
  public FieldType Type { get; init; }

  [FunctionProperty("The description of the field.")]
  public string Description { get; init; } = string.Empty;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
enum FieldType
{
  Date,
  List,
  Number,
  Text,
}