
namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class CreateRecordTool(
  IOnspringAdmin admin
) : IOnspringAdministratorTool
{
  private const string Name = "Create_Record";
  private const string Description = """
    This tool assists a user in creating a new record in an Onspring app.
    This tool requires the user to provide the id of the app in which the record will be created.
    This tool requires the user to provide the field values for the new record.
    This tool will return a link to the newly created record.
  """;
  private readonly IOnspringAdmin _admin = admin;

  public async Task<string> CreateRecord(
    [FunctionParameter("The id of the app in which the record will be created.", required: true)]
    string appId,
    [FunctionParameter("The field values for the new record.", required: true)]
    List<FieldValues> fieldValues
  )
  {
    return await _admin.PerformActionAsync(async page =>
    {
      await page.GotoAsync($"/Content/{appId}/Add");
      await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

      foreach (var field in fieldValues)
      {
        var fieldElement = page.Locator($@".label:has-text(""{field.FieldName}"") + .data").First;

        switch (field.Type)
        {
          case "Text":
            try
            {
              await fieldElement.Locator("input").FillAsync(field.Value, new() { Timeout = 3000 });
            }
            catch (Exception ex) when (ex is TimeoutException)
            {
              await fieldElement.Locator(".content-area.mce-content-body").FillAsync(field.Value);
            }
            break;
          case "List":
            try
            {
              await fieldElement.GetByRole(AriaRole.Listbox).ClickAsync(new() { Timeout = 3000 });

              var option = page.GetByRole(AriaRole.Option, new() { NameRegex = new Regex(field.Value, RegexOptions.IgnoreCase) }).First;
              await option.WaitForAsync();
              await option.ClickAsync();
            }
            catch (Exception ex) when (ex is TimeoutException)
            {
              await fieldElement.Locator(".onx-selector").ClickAsync();

              var selector = page.Locator(".selector-control:visible");
              await selector.Locator(".unselected-pane").GetByText(field.Value).ClickAsync();
              await selector.GetByTitle("Close").ClickAsync();
            }
            break;
          case "Reference" or "Parallel Reference":
            var searchResults = page.Locator("div.grid-search-results:visible");
            var searchResultRow = searchResults.GetByRole(AriaRole.Row, new() { NameRegex = new Regex(field.Value, RegexOptions.IgnoreCase) }).First;
            var scrollableElement = page.Locator(".k-grid-content.k-auto-scrollable").First;

            await fieldElement.GetByPlaceholder("Select Related").PressSequentiallyAsync(field.Value, new() { Delay = 150 });
            await page.WaitForResponseAsync(new Regex("/Content/[0-9]+/[0-9]+/EditReferenceSearchList", RegexOptions.IgnoreCase));

            var isVisible = await searchResultRow.IsVisibleAsync();

            while (isVisible is false)
            {
              await scrollableElement.EvaluateAsync("el => (el.scrollTop = el.scrollHeight)");
              isVisible = await searchResultRow.IsVisibleAsync();
            }

            await searchResultRow.GetByRole(AriaRole.Radio).ClickAsync();
            await searchResults.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("select", RegexOptions.IgnoreCase) }).ClickAsync();

            break;
        }
      }

      await page.GetByRole(AriaRole.Link, new() { NameRegex = new Regex("save record", RegexOptions.IgnoreCase) }).ClickAsync();
      await page.WaitForURLAsync(new Regex(@"/Content/[0-9]+/[0-9]+/Edit", RegexOptions.IgnoreCase));

      var url = page.Url;

      return $"The new record has been created. You can view the record [here]({url}).";
    });
  }

  public Tool Create()
  {
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(CreateRecord));
  }
}

record FieldValues(
  string FieldName,
  string Type,
  string Value
);