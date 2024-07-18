
namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class DraftAppRecordTool(
  IOnspringAdmin admin
) : IOnspringAdministratorTool
{
  private const string Name = "Draft_App_Record";
  private const string Description = """
    This tool assists a user in drafting a new record to be created in an Onspring app.
    This tool requires the user to provide the app in which the record will be created.
    This tool will return all the fields that are required to create a new record in the specified app.
  """;
  private readonly IOnspringAdmin _admin = admin;

  public async Task<string> DraftRecord(
    [FunctionParameter("The name of the app in which the record will be created.", required: true)]
    string appName
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
        return $"The app named '{appName}' was not found.";
      }

      await appRow.ClickAsync();
      await page.WaitForURLAsync(new Regex(@"/Admin/App/\d+", RegexOptions.IgnoreCase));

      await page.GetByRole(AriaRole.Tab, new() { NameRegex = new Regex("layouts", RegexOptions.IgnoreCase) }).ClickAsync();
      await page.Locator("#grid-layouts").GetByRole(AriaRole.Row, new() { NameRegex = new("default layout", RegexOptions.IgnoreCase) }).ClickAsync();
      var layoutDesigner = page.GetByRole(AriaRole.Dialog);
      await layoutDesigner.WaitForAsync();

      var frame = layoutDesigner.FrameLocator("iframe").First;
      await frame.Locator(".mainContainer").Locator(".k-tabstrip-items").WaitForAsync();
      var tabs = await frame.Locator("[data-canvas-tab]").AllAsync();

      List<RequiredField> requiredFields = [];

      foreach (var tab in tabs)
      {
        await tab.ClickAsync();
        var tabName = await tab.Locator(".tab-name").TextContentAsync();
        var tabId = await tab.GetAttributeAsync("data-canvas-tab");

        if (tabId is null)
        {
          continue;
        }

        var tabBody = frame.Locator($@"[data-tab-body=""{tabId}""]");

        var fields = await tabBody.Locator(".rendered-item.layoutItem.required").AllAsync();

        foreach (var field in fields)
        {
          var fieldName = await field.Locator(".itemName").TextContentAsync();
          var fieldType = await field.Locator(".itemType").TextContentAsync();

          if (tabName is null || fieldName is null || fieldType is null)
          {
            return "An error occurred while drafting the record. Unable to retrieve required fields.";
          }

          var tabNameTrimmed = tabName.Trim();
          var fieldNameTrimmed = fieldName.Trim();
          var fieldTypeTrimmed = fieldType.Trim();

          List<string> possibleValues = [];

          if (fieldTypeTrimmed is "List")
          {
            await field.HoverAsync();
            await field.GetByTitle("Edit Field Properties").ClickAsync();

            var fieldDialog = page.GetByRole(AriaRole.Dialog, new() { NameRegex = new Regex("field properties", RegexOptions.IgnoreCase) });

            var fieldDialogFrame = fieldDialog.FrameLocator("iframe").First;
            var listValues = fieldDialogFrame.Locator(".list-values");
            await listValues.WaitForAsync();
            var rows = await listValues.Locator(".k-grid-content")
              .Locator("tr")
              .AllAsync();

            foreach (var row in rows)
            {
              var name = await row.Locator(@"[data-cell=""name""]").Locator(".display").TextContentAsync();
              possibleValues.Add(name ?? string.Empty);
            }

            await fieldDialog.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("cancel", RegexOptions.IgnoreCase) }).ClickAsync();
          }

          var referencedApp = "Not Applicable";

          if (fieldTypeTrimmed is "Reference" or "Parallel Reference")
          {
            await field.HoverAsync();
            await field.GetByTitle("Edit Field Properties").ClickAsync();

            var fieldDialog = page.GetByRole(AriaRole.Dialog, new() { NameRegex = new Regex("field properties", RegexOptions.IgnoreCase) });
            var fieldDialogFrame = page.FrameLocator("iframe").Last;
            var referenceSetting = fieldDialogFrame.Locator(".label:has-text('Reference') + .text");
            await referenceSetting.WaitForAsync();

            var referenceText = await fieldDialogFrame.Locator(".label:has-text('Reference') + .text")
              .Locator("span")
              .First
              .TextContentAsync();

            var namePattern = @"App:\s*""([^""]+)""";
            var nameRegex = new Regex(namePattern);
            var match = nameRegex.Match(referenceText ?? string.Empty);
            referencedApp = match.Groups[1].Value;

            await fieldDialog.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("cancel", RegexOptions.IgnoreCase) }).ClickAsync();
          }

          requiredFields.Add(new RequiredField
          {
            Tab = tabName.Trim(),
            Name = fieldName.Trim(),
            Type = fieldType.Trim(),
            PossibleValues = possibleValues.Count is 0
              ? "Not Applicable"
              : string.Join(", ", possibleValues),
            ReferencedApp = referencedApp,
          });
        }
      }

      foreach (var reference in requiredFields.Where(f => f.Type is "Reference" or "Parallel Reference"))
      {
        await page.GotoAsync($"/Admin/App");
        var filterInput = page.GetByPlaceholder(new Regex("filter by", RegexOptions.IgnoreCase));

        await filterInput.ClearAsync();
        await filterInput.PressSequentiallyAsync(reference.ReferencedApp, new() { Delay = 150 });
        await page.WaitForResponseAsync(new Regex("/Admin/App/AppsListRead", RegexOptions.IgnoreCase));

        var appGrid = page.Locator("#grid");
        var scrollableElement = appGrid.Locator(".k-grid-content.k-auto-scrollable").First;

        var pager = page.Locator(".k-pager-info").First;
        var pagerText = await pager.InnerTextAsync();
        var totalNumOfApps = int.Parse(pagerText.Trim().Split(" ")[0]);
        var appRows = appGrid.GetByRole(AriaRole.Row);
        var appRowsCount = await appRows.CountAsync();

        do
        {
          await scrollableElement.EvaluateAsync("el => (el.scrollTop = el.scrollHeight)");
          await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
          appRowsCount = await appRows.CountAsync();
        } while (appRowsCount < totalNumOfApps);

        var referencedAppRow = page.GetByRole(AriaRole.Row, new() { NameRegex = new Regex(reference.ReferencedApp, RegexOptions.IgnoreCase) }).Last;
        await referencedAppRow.ScrollIntoViewIfNeededAsync();
        var isReferencedAppRowVisible = await referencedAppRow.IsVisibleAsync();

        if (isReferencedAppRowVisible is not false)
        {
          await referencedAppRow.ClickAsync();
          await page.WaitForURLAsync(new Regex(@"/Admin/App/\d+", RegexOptions.IgnoreCase));

          var displayLinkField = await page.Locator(".label:has-text('Display Link Field') + .text").TextContentAsync();
          reference.ReferencedAppDisplayField = displayLinkField?.Trim() ?? string.Empty;
        }
      }

      var msg = new StringBuilder();
      msg.AppendLine($"The following fields are required to create a new record in the {appName} app:");

      foreach (var field in requiredFields)
      {
        msg.AppendLine($"- Name: {field.Name}");
        msg.AppendLine($"  - Type: {field.Type}");
        msg.AppendLine($"  - Tab: {field.Tab}");
        msg.AppendLine($"  - Possible Values: {field.PossibleValues}");
        msg.AppendLine($"  - Referenced App: {field.ReferencedApp}");
        msg.AppendLine($"  - Referenced App Display Field: {field.ReferencedAppDisplayField}");
        msg.AppendLine();
      }

      return msg.ToString();
    });
  }

  public Tool Create()
  {
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(DraftRecord));
  }
}

class RequiredField
{
  public string Tab { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public string Type { get; set; } = string.Empty;
  public string PossibleValues { get; set; } = "Not Applicable";
  public string ReferencedApp { get; set; } = "Not Applicable";
  public string ReferencedAppDisplayField { get; set; } = "Not Applicable";
}