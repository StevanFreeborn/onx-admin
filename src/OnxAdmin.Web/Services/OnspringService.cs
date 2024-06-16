using System.Reflection;
using System.Text.RegularExpressions;

using Tool = Anthropic.SDK.Common.Tool;

namespace OnxAdmin.Web.Services;

class OnspringOptions
{
  public string InstanceUrl { get; set; } = string.Empty;
  public string CopilotUsername { get; set; } = string.Empty;
  public string CopilotPassword { get; set; } = string.Empty;
}

class OnspringOptionsSetup(IConfiguration config) : IConfigureOptions<OnspringOptions>
{
  private const string SectionName = nameof(OnspringOptions);
  private readonly IConfiguration _config = config;

  public void Configure(OnspringOptions options)
  {
    _config.GetSection(SectionName).Bind(options);
  }
}

interface IOnspringService
{
  List<Tool> GetTools();
  Task<string> GetCountOfAppsAsync(object? _ = null);
}

class OnspringService(IOptions<OnspringOptions> options) : IOnspringService, IAsyncDisposable
{
  private readonly OnspringOptions _options = options.Value;
  private IBrowser? Browser { get; set; }

  public List<Tool> GetTools()
  {
    return GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
      .Where(m => m.GetCustomAttribute<FunctionAttribute>() is not null)
      .Select(m => Tool.GetOrCreateTool(this, m.Name, m.GetCustomAttribute<FunctionAttribute>()!.Description))
      .ToList();
  }

  [Function("This function allows the user to create a new app in an Onspring instance.")]
  public async Task<string> CreateAppAsync(
    [FunctionParameter("The name of the app to create", true)] string name
  )
  {
    var browser = await CreateBrowserAsync();
    var context = await LoginAsync(browser);

    var page = await context.NewPageAsync();

    try
    {
      var addAppJson = $@"{{
      ""Id"": 0,
      ""Name"": ""{name}"",
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

      var appAppBody = JsonSerializer.Deserialize<Dictionary<string, object>>(addAppJson);

      var addAppResponse = await page.APIRequest.PostAsync("/Admin/App/AddUsingGeneralSettings", new()
      {
        Headers = new Dictionary<string, string>
        {
          ["Content-Type"] = "application/json",
        },
        DataObject = appAppBody,
      });

      if (addAppResponse.Ok is false)
      {
        throw new ToolException("Failed to create app");
      }

      var addAppResponseJson = await addAppResponse.JsonAsync();

      if (addAppResponseJson.Value.TryGetProperty("success", out var success) && success.GetBoolean() is false)
      {
        var errors = addAppResponseJson.Value.GetProperty("errors").EnumerateArray().Select(e => e.GetProperty("message").GetString()).ToList();
        var message = string.Join(Environment.NewLine, errors);
        throw new ToolException(message);
      }

      var getAppListJson = $@"{{
      ""filter"": ""{name}"",
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
        throw new ToolException("Failed to get app list");
      }

      var getAppListResponseJson = await getAppListResponse.JsonAsync();
      var apps = getAppListResponseJson.Value.GetProperty("data").EnumerateArray().ToList();

      string? appId = null;

      foreach (var app in apps)
      {
        var id = app.GetProperty("id").GetString();
        var a1 = app.GetProperty("a1").EnumerateArray().ToList()[0];
        var appName = a1.GetProperty("text").GetString();

        if (appName is not null && appName.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
          appId = id;
          break;
        }
      }

      if (string.IsNullOrEmpty(appId))
      {
        throw new ToolException("Failed to get app id");
      }

      await page.GotoAsync($"/Admin/App/{appId}");
      return page.Url;
    }
    finally
    {
      await page.CloseAsync();
      await context.CloseAsync();
    }

  }

  [Function("This function gets the count of apps in an Onspring instance")]
  public async Task<string> GetCountOfAppsAsync(
    [FunctionParameter("This is a placeholder parameter. It is not needed to call the function.", false)] object? _ = null
  )
  {
    var browser = await CreateBrowserAsync();
    var context = await LoginAsync(browser);

    var page = await context.NewPageAsync();
    var json = @"{
      ""filter"": """",
      ""hideReadOnly"": false,
      ""pageSize"": 1,
      ""requestedPage"": 1,
      ""sorting"": [
        {
          ""columnId"": ""a1"",
          ""sortDirection"": ""0""
        }
      ],
      ""addlFilterConfigs"": {
        ""gridFilterConfigs"": [],
        ""dashboardFilterConfigs"": [],
        ""addlFilterConfigs"": []
      }
    }";

    var body = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

    var response = await page.APIRequest.PostAsync("/Admin/App/AppsListRead", new()
    {
      Headers = new Dictionary<string, string>
      {
        ["Content-Type"] = "application/json",
      },
      DataObject = body,
    });

    if (response.Ok is false)
    {
      throw new ToolException("Failed to get count of apps");
    }

    var count = await response.JsonAsync();
    var totalProperty = count.Value.GetProperty("totalCount");

    if (totalProperty.TryGetInt64(out var total) is false)
    {
      throw new ToolException("Failed to get count of apps");
    }

    return total.ToString();
  }

  private async Task<IBrowserContext> LoginAsync(IBrowser browser)
  {
    var context = await browser.NewContextAsync(new()
    {
      BaseURL = _options.InstanceUrl,
    });

    var page = await context.NewPageAsync();

    var json = $@"
    {{
      ""UserName"": ""{_options.CopilotUsername}"",
      ""hiddenPassword"": ""{_options.CopilotPassword}""
    }}
    ";

    var body = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

    var loginResponse = await page.APIRequest.PostAsync("/Public/Login", new()
    {
      Headers = new Dictionary<string, string>
      {
        ["Content-Type"] = "application/json",
      },
      DataObject = body,
    });

    if (loginResponse.Ok is false)
    {
      throw new ToolException("Failed to login");
    }

    await page.CloseAsync();

    return context;
  }

  private async Task<IBrowser> CreateBrowserAsync()
  {
    if (Browser is not null)
    {
      return Browser;
    }

    var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync(new()
    {
      Headless = false,
    });

    Browser = browser;

    return browser;
  }


  public async ValueTask DisposeAsync()
  {
    if (Browser is not null)
    {
      await Browser.CloseAsync();
    }
  }
}