namespace OnxAdmin.API.Services;

interface IOnspringService
{
  Task<List<HelpCenterDocument>> GetHelpCenterDocumentsAsync();
}

class OnspringService(
  IOnspringAdmin admin,
  ILogger<OnspringService> logger
) : IOnspringService
{
  private readonly IOnspringAdmin _admin = admin;
  private readonly ILogger<OnspringService> _logger = logger;

  public async Task<List<HelpCenterDocument>> GetHelpCenterDocumentsAsync()
  {
    return await _admin.PerformActionAsync(async page =>
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
        var retries = 0;
        var maxRetries = 3;

        do
        {
          try
          {
            await page.GotoAsync(link, new() { WaitUntil = WaitUntilState.NetworkIdle });

            var main = page.Locator("#mc-main-content");
            var html = await main.InnerHTMLAsync();
            var content = new Converter(new()
            {
              UnknownTags = Config.UnknownTagsOption.PassThrough,
              GithubFlavored = true,
              RemoveComments = true,
              SmartHrefHandling = true
            }).Convert(html);

            if (string.IsNullOrWhiteSpace(content))
            {
              continue;
            }

            var pageUrl = new Uri(page.Url);
            var path = pageUrl.AbsolutePath;
            var documentTitle = path.Replace("/", "_").TrimStart('_').Replace(".htm", string.Empty);

            documents.Add(new(documentTitle, link, content));
            break;
          }
          catch (Exception ex)
          {
            retries++;
            _logger.LogError(ex, "Failed to retrieve document from {Link}. Retrying {Retries}/{MaxRetries}...", link, retries, maxRetries);
          }
        } while (retries < maxRetries);
      }

      return documents;
    });
  }
}