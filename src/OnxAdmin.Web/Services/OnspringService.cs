using System.Text.RegularExpressions;

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
  Task<string> GetCountOfAppsAsync(object? _ = null);
}

class OnspringService(IOptions<OnspringOptions> options) : IOnspringService
{
  private readonly OnspringOptions _options = options.Value;

  public async Task<string> GetCountOfAppsAsync(object? _ = null)
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
      throw new Exception("Failed to get count of apps");
    }

    var count = await response.JsonAsync();
    var totalProperty = count.Value.GetProperty("totalCount");

    if (totalProperty.TryGetInt64(out var total) is false)
    {
      throw new Exception("Failed to get count of apps");
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

    await page.GotoAsync("/Public/Login");

    await page
      .GetByPlaceholder(new Regex("Username", RegexOptions.IgnoreCase))
      .FillAsync(_options.CopilotUsername);

    await page
      .GetByPlaceholder(new Regex("Password", RegexOptions.IgnoreCase))
      .FillAsync(_options.CopilotPassword);

    await page
      .GetByText(new Regex("login", RegexOptions.IgnoreCase))
      .ClickAsync();

    await page.WaitForURLAsync(new Regex("/Dashboard", RegexOptions.IgnoreCase));
    await page.CloseAsync();

    return context;
  }

  private async Task<IBrowser> CreateBrowserAsync()
  {
    var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync();
    return browser;
  }
}