namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class FindAppByNameTool(
  IOnspringAdmin admin
) : IOnspringAdministratorTool
{
  private const string Name = "Find_App_By_Name";
  private const string Description = """
    This tool helps you find an app in Onspring by searching for its name.
    To use this tool you will need to provide the name of the app you are looking for.
    If the app is found, the tool will return the app's details.
    If the app is not found, the tool will return a message indicating that the app was not found.
  """;

  private readonly IOnspringAdmin _admin = admin;

  public async Task<string> FindAppByName(
    [FunctionParameter("The name of the app to look for.", required: true)]
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

      var appId = page.Url.Split('/').Last();

      return $"Found app named '{appName}' with ID '{appId}' at URL '{page.Url}'.";
    });
  }

  public Tool Create()
  {
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(FindAppByName));
  }
}