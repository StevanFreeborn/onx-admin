
namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class CreateSurveyTool(
  IOnspringAdmin admin
) : IOnspringAdministratorTool
{
  private const string Name = "Create_Survey";
  private const string Description = """
    This tool can be used to create a survey in Onspring. 
    It will create a survey with the given name.
    If the survey already exists, it will return message that the survey already exists.
    If the survey is created successfully, it will return message with the survey details.
  """;
  private readonly IOnspringAdmin _admin = admin;

  public async Task<string> CreateSurvey(
    [FunctionParameter("The name of the survey to create.", required: true)]
    string surveyName
  )
  {
    return await _admin.PerformActionAsync(async page =>
    {
      var createSurveyJson = $@"{{
        ""Id"": 0,
        ""CopyResponseApp"": false,
        ""Name"": ""{surveyName}"",
        ""AppStatus"": ""Enabled"",
        ""Description"": """",
        ""Heading"": """",
        ""GlobalImageId"": """",
        ""ImageId"": """",
        ""QuestionNumbering"": 0,
        ""ContentVersioning"": ""Enabled"",
        ""SaveDirectUserVersions"": true,
        ""SaveIndirectUserVersions"": false,
        ""SaveApiVersions"": false,
        ""SaveSystemVersions"": false,
        ""DisplayConcurrentEditingAlert"": false,
        ""ScheduleReportStatus"": ""Enabled""
      }}";

      var createSurveyBody = JsonSerializer.Deserialize<Dictionary<string, object>>(createSurveyJson);

      var createSurveyResponse = await page.APIRequest.PostAsync("/Admin/Survey/AddUsingGeneralSettings", new()
      {
        Headers = new Dictionary<string, string>
        {
          ["Content-Type"] = "application/json",
        },
        DataObject = createSurveyBody,
        Timeout = 60_000,
      });

      if (createSurveyResponse.Ok is false)
      {
        return "Failed to create survey";
      }

      var createSurveyResponseJson = await createSurveyResponse.JsonAsync();

      if (createSurveyResponseJson.Value.TryGetProperty("success", out var success) && success.GetBoolean() is false)
      {
        var errors = createSurveyResponseJson.Value.GetProperty("errors").EnumerateArray().Select(e => e.GetProperty("message").GetString()).ToList();
        var message = string.Join(Environment.NewLine, errors);
        return $"Unable to create survey due to the following errors:{Environment.NewLine}{message}";
      }

      var getSurveyListJson = $@"{{
        ""filter"": ""{surveyName}"",
        ""hideReadOnly"": false,
        ""pageSize"": 50,
        ""requestedPage"": 1,
        ""sorting"": [
          {{
            ""columnId"": ""a1"",
            ""sortDirection"": ""0""
          }}
        ],
        ""addlFilterConfigs"": {{
          ""gridFilterConfigs"": [],
          ""dashboardFilterConfigs"": [],
          ""addlFilterConfigs"": []
        }}
      }}";

      var getSurveyListBody = JsonSerializer.Deserialize<Dictionary<string, object>>(getSurveyListJson);

      var getSurveyListResponse = await page.APIRequest.PostAsync("/Admin/Survey/SurveyListRead", new()
      {
        Headers = new Dictionary<string, string>
        {
          ["Content-Type"] = "application/json",
        },
        DataObject = getSurveyListBody,
      });

      if (getSurveyListResponse.Ok is false)
      {
        return "Failed to get survey list after creating survey";
      }

      var getSurveyListResponseJson = await getSurveyListResponse.JsonAsync();
      var surveys = getSurveyListResponseJson.Value.GetProperty("data").EnumerateArray().ToList();

      string? surveyId = null;

      foreach (var survey in surveys)
      {
        var id = survey.GetProperty("id").GetString();
        var a1 = survey.GetProperty("a1").EnumerateArray().ToList()[0];
        var surveyName = a1.GetProperty("text").GetString();

        if (surveyName is not null && surveyName.Equals(surveyName, StringComparison.OrdinalIgnoreCase))
        {
          surveyId = id;
          break;
        }
      }

      if (string.IsNullOrEmpty(surveyId))
      {
        return "Failed to get survey id after creating survey";
      }

      await page.GotoAsync($"/Admin/Survey/{surveyId}");
      return $"Survey named {surveyName} with id {surveyId} created successfully at {page.Url}";
    });
  }

  public Tool Create()
  {
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(CreateSurvey));
  }
}