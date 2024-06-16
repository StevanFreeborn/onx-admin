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
    var browser = await playwright.Chromium.LaunchAsync();

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