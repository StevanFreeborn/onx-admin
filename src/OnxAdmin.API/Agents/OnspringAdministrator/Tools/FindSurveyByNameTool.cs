namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class FindSurveyByNameTool(
  IOnspringAdmin admin
) : IOnspringAdministratorTool
{
  private const string Name = "Find_Survey_By_Name";
  private const string Description = """
    This tool helps you find a survey in Onspring by searching for its name.
    To use this tool you will need to provide the name of the survey you are looking for.
    If the survey is found, the tool will return the survey's details.
    If the survey is not found, the tool will return a message indicating that the survey was not found.
  """;

  private readonly IOnspringAdmin _admin = admin;

  public async Task<string> FindAppByName(
    [FunctionParameter("The name of the survey to look for.", required: true)]
    string surveyName
  )
  {
    return await _admin.PerformActionAsync(async page =>
    {
      await page.GotoAsync($"/Admin/Survey");
      await page.GetByPlaceholder(new Regex("filter by", RegexOptions.IgnoreCase)).PressSequentiallyAsync(surveyName, new() { Delay = 150 });
      await page.WaitForResponseAsync(new Regex("/Admin/Survey/SurveyListRead", RegexOptions.IgnoreCase));

      var surveyRow = page.GetByRole(AriaRole.Row, new() { NameRegex = new Regex(surveyName, RegexOptions.IgnoreCase), Exact = true }).First;
      var isRowVisible = await surveyRow.IsVisibleAsync();

      if (isRowVisible is false)
      {
        return $"The survey named '{surveyName}' was not found.";
      }

      await surveyRow.ClickAsync();
      await page.WaitForURLAsync(new Regex(@"/Admin/Survey/\d+", RegexOptions.IgnoreCase));

      var surveyId = page.Url.Split('/').Last();

      return $"Found survey named '{surveyName}' with ID '{surveyId}' at URL '{page.Url}'.";
    });
  }

  public Tool Create()
  {
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(FindAppByName));
  }
}