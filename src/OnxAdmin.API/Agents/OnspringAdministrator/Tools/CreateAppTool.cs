
namespace OnxAdmin.API.Agents.OnspringAdministrator.Tools;

class CreateAppTool(
  IOnspringAdmin admin
) : IOnspringAdministratorTool
{
  private const string Name = "Create_App";
  private const string Description = """
    This tool can be used to create an app in Onspring. 
    It will create an app with the given name.
    If the app already exists, it will return message that the app already exists.
    If the app is created successfully, it will return message with the apps details.
  """;
  private readonly IOnspringAdmin _admin = admin;

  public async Task<string> CreateApp(
    [FunctionParameter("The name of the app to create.", required: true)]
    string appName
  )
  {
    return await _admin.PerformActionAsync(async page =>
    {
      var createAppJson = $@"{{
        ""Id"": 0,
        ""Name"": ""{appName}"",
        ""AppStatus"": ""Enabled"",
        ""Description"": """",
        ""GlobalImageId"": """",
        ""ImageId"": """",
        ""ContentVersioning"": ""Enabled"",
        ""SaveDirectUserVersions"": true,
        ""SaveIndirectUserVersions"": false,
        ""SaveApiVersions"": false,
        ""SaveSystemVersions"": false,
        ""DisplayConcurrentEditingAlert"": true
      }}";

      var createAppBody = JsonSerializer.Deserialize<Dictionary<string, object>>(createAppJson);

      var createAppResponse = await page.APIRequest.PostAsync("/Admin/App/AddUsingGeneralSettings", new()
      {
        Headers = new Dictionary<string, string>
        {
          ["Content-Type"] = "application/json",
        },
        DataObject = createAppBody,
      });

      if (createAppResponse.Ok is false)
      {
        return "Failed to create app";
      }

      var createAppResponseJson = await createAppResponse.JsonAsync();

      if (createAppResponseJson.Value.TryGetProperty("success", out var success) && success.GetBoolean() is false)
      {
        var errors = createAppResponseJson.Value.GetProperty("errors").EnumerateArray().Select(e => e.GetProperty("message").GetString()).ToList();
        var message = string.Join(Environment.NewLine, errors);
        return $"Unable to create app due to the following errors:{Environment.NewLine}{message}";
      }

      var getAppListJson = $@"{{
        ""filter"": ""{appName}"",
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

      var getAppListBody = JsonSerializer.Deserialize<Dictionary<string, object>>(getAppListJson);

      var getAppListResponse = await page.APIRequest.PostAsync("/Admin/App/AppsListRead", new()
      {
        Headers = new Dictionary<string, string>
        {
          ["Content-Type"] = "application/json",
        },
        DataObject = getAppListBody,
      });

      if (getAppListResponse.Ok is false)
      {
        return "Failed to get app list after creating app";
      }

      var getAppListResponseJson = await getAppListResponse.JsonAsync();
      var apps = getAppListResponseJson.Value.GetProperty("data").EnumerateArray().ToList();

      string? appId = null;

      foreach (var app in apps)
      {
        var id = app.GetProperty("id").GetString();
        var a1 = app.GetProperty("a1").EnumerateArray().ToList()[0];
        var appName = a1.GetProperty("text").GetString();

        if (appName is not null && appName.Equals(appName, StringComparison.OrdinalIgnoreCase))
        {
          appId = id;
          break;
        }
      }

      if (string.IsNullOrEmpty(appId))
      {
        return "Failed to get app id after creating app";
      }

      await page.GotoAsync($"/Admin/App/{appId}");
      return $"App named {appName} with id {appId} created successfully at {page.Url}";
    });
  }

  public Tool Create()
  {
    return Tool.CreateFromInstanceMethod(Name, Description, this, nameof(CreateApp));
  }
}