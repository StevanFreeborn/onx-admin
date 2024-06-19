#pragma warning disable SKEXP0001

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

[JsonConverter(typeof(JsonStringEnumConverter))]
enum FieldType
{
  Date,
  List,
  Number,
  Text,
}

interface IOnspringService
{
  List<Tool> GetTools();
  Task<List<HelpCenterDocument>> GetHelpCenterDocumentsAsync();
}

class OnspringService(IOptions<OnspringOptions> options, ISemanticTextMemory memory, ILogger<OnspringService> logger) : IOnspringService, IDisposable, IAsyncDisposable
{
  private readonly OnspringOptions _options = options.Value;
  private readonly ISemanticTextMemory _memory = memory;
  private readonly ILogger<OnspringService> _logger = logger;
  private IBrowser? Browser { get; set; }

  public List<Tool> GetTools()
  {
    return GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
      .Where(m => m.GetCustomAttribute<FunctionAttribute>() is not null)
      .Select(m => Tool.GetOrCreateTool(this, m.Name, m.GetCustomAttribute<FunctionAttribute>()!.Description))
      .ToList();
  }

  [Function("This function allows the user to create a new field in an app in an Onspring instance. It will return the URL of the app where the field was created.")]
  public async Task<string> CreateFieldAsync(
    [FunctionParameter("The name of the app where the field should be created create", true)] 
    string appName,
    [FunctionParameter("The name of the field to create", true)] 
    string name,
    [FunctionParameter("The type of the field to create", true)] 
    FieldType type,
    [FunctionParameter("The description of the field to create", false)] 
    string description = ""
  )
  {
    return await PerformActionAsync(async page =>
    {
      await page.GotoAsync($"/Admin/App");
      await page.GetByPlaceholder(new Regex("filter by", RegexOptions.IgnoreCase)).PressSequentiallyAsync(appName, new() { Delay = 150 });
      await page.WaitForResponseAsync(new Regex("/Admin/App/AppsListRead", RegexOptions.IgnoreCase));

      var appRow = page.GetByRole(AriaRole.Row, new() { NameRegex = new Regex(appName, RegexOptions.IgnoreCase) });
      var isRowVisible = await appRow.IsVisibleAsync();

      if (isRowVisible is false)
      {
        throw new ToolException("App not found");
      }

      await appRow.ClickAsync();
      await page.WaitForURLAsync(new Regex(@"/Admin/App/\d+", RegexOptions.IgnoreCase));
      await page.GetByRole(AriaRole.Tab, new() { NameRegex = new("layouts", RegexOptions.IgnoreCase) }).ClickAsync();

      await page.Locator("[data-add-button='layout-item']").ClickAsync();

      var layoutItemMenu = page.Locator("[data-add-menu='layout-item']");
      await layoutItemMenu.WaitForAsync();
      await layoutItemMenu.GetByText(new Regex($"{type}", RegexOptions.IgnoreCase)).First.ClickAsync();

      var addFieldDialog = page.GetByRole(AriaRole.Dialog, new() { NameRegex = new($"add.*field", RegexOptions.IgnoreCase) });
      await addFieldDialog.WaitForAsync();
      await addFieldDialog.GetByRole(AriaRole.Button, new() { NameRegex = new("continue", RegexOptions.IgnoreCase) }).ClickAsync();
      await addFieldDialog.WaitForAsync();

      var frame = addFieldDialog.FrameLocator("iframe");
      await frame.Locator(".label:has-text('Field') + .data input").FillAsync(name);
      await frame.Locator(".label:has-text('Description') + .data .content-area.mce-content-body").FillAsync(description);
      await addFieldDialog.GetByRole(AriaRole.Button, new() { NameRegex = new("save", RegexOptions.IgnoreCase) }).ClickAsync();

      var addFieldResponse = await page.WaitForResponseAsync(new Regex(@"/Admin/App/\d+/Field/AddUsingSettings", RegexOptions.IgnoreCase));

      if (addFieldResponse.Ok is false)
      {
        throw new ToolException("Failed to create field");
      }

      var addFieldResponseJson = await addFieldResponse.JsonAsync();

      if (addFieldResponseJson.Value.TryGetProperty("success", out var success) && success.GetBoolean() is false)
      {
        var errors = addFieldResponseJson.Value.GetProperty("errors").EnumerateArray().Select(e => e.GetProperty("message").GetString()).ToList();
        var message = string.Join(Environment.NewLine, errors);
        throw new ToolException(message);
      }

      return page.Url;
    });
  }

  [Function("This function allows the user to create a new app in an Onspring instance. It will return the URL of the app that was created.")]
  public async Task<string> CreateAppAsync(
    [FunctionParameter("The name of the app to create", true)] 
    string name
  )
  {
    return await PerformActionAsync(async page =>
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
    });
  }

  [Function("This function gets the count of apps in an Onspring instance. It will return the number of apps in the instance.")]
  public async Task<string> GetCountOfAppsAsync(
    [FunctionParameter("This is a placeholder parameter. It is not needed to call the function.", false)] 
    object? _ = null
  )
  {
    return await PerformActionAsync(async page =>
    {
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

      var responseBody = await response.JsonAsync();
      var totalProperty = responseBody.Value.GetProperty("totalCount");

      if (totalProperty.TryGetInt64(out var total) is false)
      {
        throw new ToolException("Failed to get count of apps");
      }

      return total.ToString();
    });
  }

  [Function("This function can be used to query the help center documents in an Onspring instance. It will return the documents that match the query. The documents can be used to help answer questions.")]
  public async Task<string> QueryHelpCenterDocuments(
    [FunctionParameter("The query to search for in the help center documents", true)]
    string query
  )
  {
    try
    {
      var results = _memory.SearchAsync(
        collection: HelpCenterGenerateEmbeddingsService.HelpCenterCollection,
        query: $"search_query: {query}",
        limit: 10,
        minRelevanceScore: 0
      );

      var knowledge = new StringBuilder();

      await foreach (var result in results)
      {
        var text = result.Metadata.Text.Replace("search_document: ", string.Empty);
        knowledge.AppendLine(text);
      }

      return knowledge.ToString();
    }
    catch (Exception ex)
    {
      throw new ToolException("Failed to query help center documents", ex);
    }
    
  }

  public async Task<List<HelpCenterDocument>> GetHelpCenterDocumentsAsync()
  {
    return await PerformActionAsync(async page =>
    {
      await page.GotoAsync("/Help/Content/Home.htm", new() { WaitUntil = WaitUntilState.NetworkIdle });

      var navItems = page.Locator(".sidenav li.tree-node:not(.tree-node-leaf)");

      for (var i = 0; i < await navItems.CountAsync(); i++)
      {
        var navItem = navItems.Nth(i);
        var toggle = navItem.Locator(".submenu-toggle-container");
        var ariaExpanded = await toggle.GetAttributeAsync("aria-expanded");
        var isExpanded = ariaExpanded == "true";

        if (isExpanded)
        {
          continue;
        }

        await navItem.ClickAsync();
      }

      var navLinks = page.Locator(".sidenav a");
      var linkAddresses = new List<string>();

      foreach (var navLink in await navLinks.AllAsync())
      {
        var href = await navLink.GetAttributeAsync("href");

        if (href is null || href.Contains("javascript:void(0)"))
        {
          continue;
        }

        linkAddresses.Add(href);
      }

      var linkAbsoluteAddresses = linkAddresses.Select(link => link.Replace("..", "/Help")).ToList();

      var documents = new List<HelpCenterDocument>();

      foreach (var link in linkAbsoluteAddresses)
      {
        try
        {
          await page.GotoAsync(link, new() { WaitUntil = WaitUntilState.NetworkIdle });

          var main = page.Locator("#mc-main-content");
          var content = await main.InnerTextAsync();

          if (string.IsNullOrWhiteSpace(content))
          {
            continue;
          }

          var cleanedContent = Regex.Replace(content, @"^\s*$\n|\r", "", RegexOptions.Multiline);
          var pageUrl = new Uri(page.Url);
          var path = pageUrl.AbsolutePath;
          var documentTitle = path.Replace("/", "_").TrimStart('_').Replace(".htm", string.Empty);

          documents.Add(new()
          {
            Title = documentTitle,
            Content = cleanedContent,
          });
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Failed to get document content");
        }
      }

      return documents;
    });
  }

  private async Task<T> PerformActionAsync<T>(Func<IPage, Task<T>> action)
  {
    var browser = await CreateBrowserAsync();
    var context = await LoginAsync(browser);

    var page = await context.NewPageAsync();

    try
    {
      return await action(page);
    }
    finally
    {
      await page.CloseAsync();
      await context.CloseAsync();
    }
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
        throw new ToolException("Failed to login");
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

  public void Dispose()
  {
    if (Browser is not null)
    {
      Browser.CloseAsync().GetAwaiter().GetResult();
    }
  }
}