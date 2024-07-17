namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class CreateSurveyQuestionsTool(
  IOnspringAdmin admin
) : IOnspringAdministratorTool
{
  private const string Name = "Create_Survey_Questions";
  private const string Description = @"
    This tool can be used to create survey questions in an Onspring survey. 
    It will create the questions provided.
    It will return a message indicating which questions were created successfully and which questions failed to be created.
    The message will also include a link to preview the survey where the questions were created.
  ";
  private readonly IOnspringAdmin _admin = admin;

  public async Task<string> CreateQuestions(
    [FunctionParameter("The name of the survey where the questions should be created.", required: true)]
    string surveyName,
    [FunctionParameter("The questions to create.", required: true)]
    List<Question> questions
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
        return $"Unable to create questions because the survey {surveyName} was not found.";
      }

      await surveyRow.ClickAsync();
      await page.WaitForURLAsync(new Regex(@"/Admin/Survey/\d+", RegexOptions.IgnoreCase));
      await page.GetByRole(AriaRole.Tab, new() { NameRegex = new("design", RegexOptions.IgnoreCase) }).ClickAsync();
      await page.GetByRole(AriaRole.Link, new() { NameRegex = new("design survey", RegexOptions.IgnoreCase) }).ClickAsync();

      var dialog = page.GetByRole(AriaRole.Dialog, new() { NameRegex = new("survey designer", RegexOptions.IgnoreCase) });
      await dialog.WaitForAsync();

      var designer = dialog.FrameLocator("iframe");

      var msg = new StringBuilder();

      foreach (var question in questions)
      {
        ILocator? button = null;

        switch (question.Type)
        {
          case QuestionType.MultiSelect:
            button = designer.GetByRole(AriaRole.Button, new() { NameRegex = new("Multi-Select", RegexOptions.IgnoreCase) }).First;
            break;
          case QuestionType.SingleLineText or QuestionType.MultiLineText:
            button = designer.GetByRole(AriaRole.Button, new() { NameRegex = new("Text", RegexOptions.IgnoreCase), Exact = true }).First;
            break;
          case QuestionType.SingleSelect:
            button = designer.GetByRole(AriaRole.Button, new() { NameRegex = new("Single Select", RegexOptions.IgnoreCase) }).First;
            break;
          case QuestionType.Date:
            button = designer.GetByRole(AriaRole.Button, new() { NameRegex = new("Date", RegexOptions.IgnoreCase) }).First;
            break;
          case QuestionType.Number:
            button = designer.GetByRole(AriaRole.Button, new() { NameRegex = new("Number", RegexOptions.IgnoreCase) }).First;
            break;
          default:
            break;
        }

        if (button is null)
        {
          msg.AppendLine($"Failed to create question: {question.Text}. The question type {question.Type} is not supported.");
          continue;
        }

        await button.ClickAsync();
        await designer.Locator("#record-status .animation").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

        // wait for the question text input to be focused
        await page.WaitForFunctionAsync(""" 
            () => {
              const frame = document.querySelector('iframe');
              const activeElement = frame?.contentWindow?.document.activeElement;
              return activeElement?.id === 'QuestionText';
            }
          """,
          null,
          new() { Timeout = 15_000 }
        );

        // wait for animation to complete
        var helpTextEditor = designer.Locator(".content-area.mce-content-body").Last;
        var helpTextEditorHandle = await helpTextEditor.ElementHandleAsync();
        await helpTextEditorHandle.WaitForElementStateAsync(ElementState.Stable);

        await designer.GetByLabel("Question Text").FillAsync(question.Text);
        await designer.GetByLabel("Question Id").FillAsync(Guid.NewGuid().ToString());

        if (question.Type is QuestionType.MultiLineText)
        {
          await designer.Locator(@".label:has-text(""Answer Option"") + .data").GetByRole(AriaRole.Listbox).ClickAsync();
          await designer.GetByRole(AriaRole.Option, new() { NameRegex = new("multi-line", RegexOptions.IgnoreCase) }).ClickAsync();
        }

        if (question.Type is QuestionType.MultiSelect or QuestionType.SingleSelect)
        {
          var listValuesGrid = designer.Locator(".list-values").First;

          foreach (var option in question.Options)
          {
            await listValuesGrid.GetByRole(AriaRole.Button, new() { NameRegex = new("add value", RegexOptions.IgnoreCase) }).ClickAsync();

            var gridBody = listValuesGrid.Locator("div.k-grid-content");
            var lastRow = gridBody.GetByRole(AriaRole.Row).Last;
            var rowValueInput = lastRow.Locator(@"td[data-field=""name""] input");
            await rowValueInput.FillAsync(option);
          }
        }

        msg.AppendLine($"Successfully created question with text: {question.Text}");
      }

      var previewPage = await page.Context.RunAndWaitForPageAsync(async () =>
      {
        var previewButton = designer.GetByRole(AriaRole.Link, new() { NameRegex = new("preview", RegexOptions.IgnoreCase) }).First;
        await previewButton.ClickAsync();
      });

      await previewPage.WaitForLoadStateAsync(LoadState.NetworkIdle);

      msg.AppendLine($"You can preview the survey with the questions created at {previewPage.Url}");

      return msg.ToString();
    });
  }

  public Tool Create()
  {
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(CreateQuestions));
  }
}

record Question
{
  [FunctionProperty("The text of the question", required: true)]
  public string Text { get; init; } = string.Empty;

  [FunctionProperty("The type of the question", required: true)]
  public QuestionType Type { get; init; }

  [FunctionProperty("The possible answers or options for the question (if applicable)")]
  public List<string> Options { get; init; } = [];
}

[JsonConverter(typeof(JsonStringEnumConverter))]
enum QuestionType
{
  MultiSelect,
  MultiLineText,
  SingleLineText,
  SingleSelect,
  Date,
  Number
}