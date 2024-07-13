namespace OnxAdmin.API.Factories;

interface IPageFactory
{
  Task<IOnspringPage> CreatePageAsync();
}

class PageFactory(IOptions<OnspringOptions> options) : IPageFactory, IAsyncDisposable
{
  private readonly OnspringOptions _options = options.Value;
  private IBrowser? Browser { get; set; }

  public async Task<IOnspringPage> CreatePageAsync()
  {
    var browser = await CreateBrowserAsync();
    var context = await LoginAsync(browser);
    var page = await context.NewPageAsync();
    return new OnspringPage() { Page = page };
  }

  private async Task<IBrowserContext> LoginAsync(IBrowser browser)
  {
    var context = await browser.NewContextAsync(new()
    {
      BaseURL = _options.InstanceUrl,
    });

    var page = await context.NewPageAsync();

    try
    {
      var json = $@"{{
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

      var text = await loginResponse.TextAsync();
      await page.SetContentAsync(text);
      var title = await page.TitleAsync();

      if (new Regex("login", RegexOptions.IgnoreCase).IsMatch(title))
      {
        throw new ApplicationException("Failed to login");
      }

      return context;
    }
    finally
    {
      await page.CloseAsync();
    }

  }

  private async Task<IBrowser> CreateBrowserAsync()
  {
    if (Browser is not null)
    {
      return Browser;
    }

    var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });

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