
namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class DataImportTool(
  IOnspringAdmin admin,
  IAttachmentService attachmentService
) : IOnspringAdministratorTool
{
  private const string Name = "Create_And_Run_Data_Import";
  private const string Description = """
    This tool can be used to create and run a data import in Onspring.
    It will create an import configuration, upload the import file, and run the import.
    It will then create a report with the imported data.
    The tool will return a message indicating that the import has completed successfully and provide a link to the report with the imported data.
  """;
  private readonly IOnspringAdmin _admin = admin;
  private readonly IAttachmentService _attachmentService = attachmentService;

  public async Task<string> CreateAndRunImport(
    [FunctionParameter("The name of the app where the data should be imported", required: true)]
    string appName,
    [FunctionParameter("The ID of the import file", required: true)]
    string importFileId,
    [FunctionParameter("The list of fields data is being imported into", required: true)]
    List<Field> importFields
  )
  {
    return await _admin.PerformActionAsync(async page =>
    {
      var msg = new StringBuilder();

      var importTime = DateTimeOffset.Now;
      var importResult = await CreateImport(page, appName, importFileId, importTime);
      msg.AppendLine(importResult);

      bool isComplete;

      do
      {
        isComplete = await CheckIfImportHasCompletedAsync(page, importTime);
        await Task.Delay(5000);
      } while (isComplete is false);

      msg.AppendLine("The import has completed successfully.");

      var reportResult = await CreateReport(page, appName, importFields);
      msg.AppendLine(reportResult);

      var url = page.Url;

      return url;
    });
  }

  private async Task<string> CreateReport(IPage page, string appName, List<Field> importFields)
  {
    await page.GotoAsync("/Report");

    await page.GetByRole(AriaRole.Button, new() { NameRegex = new("create report", RegexOptions.IgnoreCase) }).ClickAsync();
    await page.Locator("#create-menu").GetByText(appName, new() { Exact = true }).ClickAsync();

    var addReportDialog = page.GetByRole(AriaRole.Dialog, new() { NameRegex = new("add new report", RegexOptions.IgnoreCase) });
    await addReportDialog.WaitForAsync();
    await addReportDialog.GetByRole(AriaRole.Radio, new() { NameRegex = new("create as a saved report", RegexOptions.IgnoreCase) }).ClickAsync();
    await addReportDialog.GetByPlaceholder(new Regex("report name", RegexOptions.IgnoreCase)).FillAsync("All Records");
    await addReportDialog.Locator(".label:has-text('Security') + .data").GetByRole(AriaRole.Listbox).ClickAsync();
    await page.GetByRole(AriaRole.Option, new() { NameRegex = new("public", RegexOptions.IgnoreCase) }).ClickAsync();
    await addReportDialog.GetByRole(AriaRole.Button, new() { NameRegex = new("save", RegexOptions.IgnoreCase) }).ClickAsync();

    var reportDesigner = page.GetByRole(AriaRole.Dialog, new() { NameRegex = new("report designer", RegexOptions.IgnoreCase) });
    await reportDesigner.WaitForAsync();

    foreach (var field in importFields)
    {
      var frame = reportDesigner.FrameLocator("iframe");
      var fieldToDrag = frame.Locator(".item-lists").Locator($".layoutItem.draggable:has-text('{field.Name}')").First;
      var dropZone = frame.Locator(".display-field-container").Locator("[data-column]").Last;
      await fieldToDrag.DragToAsync(dropZone);
    }

    await reportDesigner.GetByRole(AriaRole.Button, new() { NameRegex = new("save changes & run", RegexOptions.IgnoreCase) }).ClickAsync();
    await page.WaitForURLAsync(new Regex(@"/Report/\d+/Display"));

    return $"The imported data can be found at {page.Url}";
  }

  private async Task<bool> CheckIfImportHasCompletedAsync(IPage page, DateTimeOffset importTime)
  {
    await page.GotoAsync("/Admin/Reporting/Messaging/History", new() { WaitUntil = WaitUntilState.NetworkIdle });

    var grid = page.Locator(".k-grid-content");
    var firstRow = grid.GetByRole(AriaRole.Row).First;

    var toCell = firstRow.GetByRole(AriaRole.Gridcell).Nth(1);
    var subjectCell = firstRow.GetByRole(AriaRole.Gridcell).Nth(4);
    var timeCreatedCell = firstRow.GetByRole(AriaRole.Gridcell).Nth(5);

    var toCellText = await toCell.TextContentAsync();
    var subjectText = await subjectCell.TextContentAsync();
    var timeCreated = await timeCreatedCell.TextContentAsync();

    var timeCreatedDate = DateTimeOffset.TryParse(timeCreated, out var date) ? date : DateTimeOffset.MinValue;

    var importTimeWithoutMilliseconds = new DateTimeOffset(
      importTime.Year,
      importTime.Month,
      importTime.Day,
      importTime.Hour,
      importTime.Minute,
      0,
      importTime.Offset
    );

    return subjectText is not null &&
      toCellText is not null &&
      subjectText.Contains("Onspring Data Import Complete", StringComparison.OrdinalIgnoreCase) &&
      timeCreatedDate >= importTimeWithoutMilliseconds &&
      toCellText.Contains("Onspring CoPilot", StringComparison.OrdinalIgnoreCase);
  }

  private async Task<string> CreateImport(IPage page, string appName, string importFileId, DateTimeOffset importTime)
  {
    await page.GotoAsync("/Admin/Home");
    await page.Locator("#admin-create-button").HoverAsync();
    await page.Locator("#admin-create-menu").Locator("[data-dialog-function='showAddImportConfig']").ClickAsync();

    var addImportDialog = page.GetByRole(AriaRole.Dialog, new() { NameRegex = new("create import configuration", RegexOptions.IgnoreCase) });
    await addImportDialog.WaitForAsync();

    var importName = $"{importTime.ToUnixTimeMilliseconds()}_{appName}_import";
    await addImportDialog.Locator("#Name").FillAsync(importName);
    await addImportDialog.GetByRole(AriaRole.Button, new() { NameRegex = new("save", RegexOptions.IgnoreCase) }).ClickAsync();
    await page.WaitForURLAsync(new Regex(@"/Admin/Integration/Import/\d+/Edit"));

    var importUrl = page.Url;

    await page.Locator(".label:has-text('App/Survey') + .data").GetByRole(AriaRole.Listbox).ClickAsync();
    await page.GetByRole(AriaRole.Option, new() { NameRegex = new(appName, RegexOptions.IgnoreCase), Exact = true }).ClickAsync();

    var fileChooser = await page.RunAndWaitForFileChooserAsync(
      async () => await page.Locator(".k-upload-button").ClickAsync()
    );

    var importFile = await _attachmentService.GetAttachmentAsync(importFileId);

    await fileChooser.SetFilesAsync(new FilePayload()
    {
      Name = importFile.FileName,
      MimeType = importFile.ContentType,
      Buffer = importFile.Stream.ToArray(),
    });

    await page.WaitForResponseAsync(new Regex(@"/Admin/Integration/Import/SaveImportFiles"));

    await page.GetByRole(AriaRole.Tab, new() { NameRegex = new("integration settings", RegexOptions.IgnoreCase) }).ClickAsync();

    var recordHandlingSelector = page.Locator(".label:has-text('Record Handling') + .data").GetByRole(AriaRole.Listbox);
    await recordHandlingSelector.ClickAsync();
    await page.GetByRole(AriaRole.Option, new() { NameRegex = new("update content that matches and add new content", RegexOptions.IgnoreCase) }).ClickAsync();

    var listConfigurationGrid = page.Locator(".label:has-text('List Configuration') + .data").Locator("table tbody");
    var listConfigurationRows = await listConfigurationGrid.GetByRole(AriaRole.Row).AllAsync();

    foreach (var row in listConfigurationRows)
    {
      var lastCell = row.GetByRole(AriaRole.Cell).Last;
      await lastCell.GetByRole(AriaRole.Listbox).ClickAsync();
      await page.GetByRole(AriaRole.Option, new() { NameRegex = new("Add any new values found to the existing list", RegexOptions.IgnoreCase) }).ClickAsync();
    }

    await recordHandlingSelector.ClickAsync();
    await page.GetByRole(AriaRole.Option, new() { NameRegex = new("Add new content for each record in the file", RegexOptions.IgnoreCase) }).ClickAsync();

    await page.GetByRole(AriaRole.Link, new() { NameRegex = new("save changes & run", RegexOptions.IgnoreCase) }).ClickAsync();

    var runDialog = page.GetByRole(AriaRole.Dialog, new() { NameRegex = new("run", RegexOptions.IgnoreCase) });
    await runDialog.WaitForAsync();
    await runDialog.GetByRole(AriaRole.Button, new() { NameRegex = new("run", RegexOptions.IgnoreCase) }).ClickAsync();
    await page.WaitForURLAsync(new Regex(@"/Admin/Integration/Import/\d+/Processing"));

    return $"The import has been created at {importUrl}";
  }

  public Tool Create()
  {
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(CreateAndRunImport));
  }
}