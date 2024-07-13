namespace OnxAdmin.API.Models;

interface IOnspringPage : IPage
{
  public IPage Page { get; init; }
}

class OnspringPage : IOnspringPage
{
  public IPage Page { get; init; } = null!;
  public IClock Clock => Page.Clock;

  [Obsolete]
  public IAccessibility Accessibility => Page.Accessibility;
  public IBrowserContext Context => Page.Context;
  public IReadOnlyList<IFrame> Frames => Page.Frames;
  public bool IsClosed => Page.IsClosed;
  public IKeyboard Keyboard => Page.Keyboard;
  public IFrame MainFrame => Page.MainFrame;
  public IMouse Mouse => Page.Mouse;
  public IAPIRequestContext APIRequest => Page.APIRequest;
  public ITouchscreen Touchscreen => Page.Touchscreen;
  public string Url => Page.Url;
  public IVideo? Video => Page.Video;
  public PageViewportSizeResult? ViewportSize => Page.ViewportSize;
  public IReadOnlyList<IWorker> Workers => Page.Workers;
  public event EventHandler<IPage> Close
  {
    add
    {
      Page.Close += value;
    }

    remove
    {
      Page.Close -= value;
    }
  }

  public event EventHandler<IConsoleMessage> Console
  {
    add
    {
      Page.Console += value;
    }

    remove
    {
      Page.Console -= value;
    }
  }

  public event EventHandler<IPage> Crash
  {
    add
    {
      Page.Crash += value;
    }

    remove
    {
      Page.Crash -= value;
    }
  }

  public event EventHandler<IDialog> Dialog
  {
    add
    {
      Page.Dialog += value;
    }

    remove
    {
      Page.Dialog -= value;
    }
  }

  public event EventHandler<IPage> DOMContentLoaded
  {
    add
    {
      Page.DOMContentLoaded += value;
    }

    remove
    {
      Page.DOMContentLoaded -= value;
    }
  }

  public event EventHandler<IDownload> Download
  {
    add
    {
      Page.Download += value;
    }

    remove
    {
      Page.Download -= value;
    }
  }

  public event EventHandler<IFileChooser> FileChooser
  {
    add
    {
      Page.FileChooser += value;
    }

    remove
    {
      Page.FileChooser -= value;
    }
  }

  public event EventHandler<IFrame> FrameAttached
  {
    add
    {
      Page.FrameAttached += value;
    }

    remove
    {
      Page.FrameAttached -= value;
    }
  }

  public event EventHandler<IFrame> FrameDetached
  {
    add
    {
      Page.FrameDetached += value;
    }

    remove
    {
      Page.FrameDetached -= value;
    }
  }

  public event EventHandler<IFrame> FrameNavigated
  {
    add
    {
      Page.FrameNavigated += value;
    }

    remove
    {
      Page.FrameNavigated -= value;
    }
  }

  public event EventHandler<IPage> Load
  {
    add
    {
      Page.Load += value;
    }

    remove
    {
      Page.Load -= value;
    }
  }

  public event EventHandler<string> PageError
  {
    add
    {
      Page.PageError += value;
    }

    remove
    {
      Page.PageError -= value;
    }
  }

  public event EventHandler<IPage> Popup
  {
    add
    {
      Page.Popup += value;
    }

    remove
    {
      Page.Popup -= value;
    }
  }

  public event EventHandler<IRequest> Request
  {
    add
    {
      Page.Request += value;
    }

    remove
    {
      Page.Request -= value;
    }
  }

  public event EventHandler<IRequest> RequestFailed
  {
    add
    {
      Page.RequestFailed += value;
    }

    remove
    {
      Page.RequestFailed -= value;
    }
  }

  public event EventHandler<IRequest> RequestFinished
  {
    add
    {
      Page.RequestFinished += value;
    }

    remove
    {
      Page.RequestFinished -= value;
    }
  }

  public event EventHandler<IResponse> Response
  {
    add
    {
      Page.Response += value;
    }

    remove
    {
      Page.Response -= value;
    }
  }

  public event EventHandler<IWebSocket> WebSocket
  {
    add
    {
      Page.WebSocket += value;
    }

    remove
    {
      Page.WebSocket -= value;
    }
  }

  public event EventHandler<IWorker> Worker
  {
    add
    {
      Page.Worker += value;
    }

    remove
    {
      Page.Worker -= value;
    }
  }

  public Task AddInitScriptAsync(string? script = null, string? scriptPath = null)
  {
    return Page.AddInitScriptAsync(script, scriptPath);
  }

  public Task AddLocatorHandlerAsync(ILocator locator, Func<ILocator, Task> handler, PageAddLocatorHandlerOptions? options = null)
  {
    return Page.AddLocatorHandlerAsync(locator, handler, options);
  }

  public Task AddLocatorHandlerAsync(ILocator locator, Func<Task> handler, PageAddLocatorHandlerOptions? options = null)
  {
    return Page.AddLocatorHandlerAsync(locator, handler, options);
  }

  public Task<IElementHandle> AddScriptTagAsync(PageAddScriptTagOptions? options = null)
  {
    return Page.AddScriptTagAsync(options);
  }

  public Task<IElementHandle> AddStyleTagAsync(PageAddStyleTagOptions? options = null)
  {
    return Page.AddStyleTagAsync(options);
  }

  public Task BringToFrontAsync()
  {
    return Page.BringToFrontAsync();
  }

  public Task CheckAsync(string selector, PageCheckOptions? options = null)
  {
    return Page.CheckAsync(selector, options);
  }

  public Task ClickAsync(string selector, PageClickOptions? options = null)
  {
    return Page.ClickAsync(selector, options);
  }

  public Task CloseAsync(PageCloseOptions? options = null)
  {
    return Page.CloseAsync(options);
  }

  public Task<string> ContentAsync()
  {
    return Page.ContentAsync();
  }

  public Task DblClickAsync(string selector, PageDblClickOptions? options = null)
  {
    return Page.DblClickAsync(selector, options);
  }

  public Task DispatchEventAsync(string selector, string type, object? eventInit = null, PageDispatchEventOptions? options = null)
  {
    return Page.DispatchEventAsync(selector, type, eventInit, options);
  }

  public Task DragAndDropAsync(string source, string target, PageDragAndDropOptions? options = null)
  {
    return Page.DragAndDropAsync(source, target, options);
  }

  public Task EmulateMediaAsync(PageEmulateMediaOptions? options = null)
  {
    return Page.EmulateMediaAsync(options);
  }

  public Task<T> EvalOnSelectorAllAsync<T>(string selector, string expression, object? arg = null)
  {
    return Page.EvalOnSelectorAllAsync<T>(selector, expression, arg);
  }

  public Task<JsonElement?> EvalOnSelectorAllAsync(string selector, string expression, object? arg = null)
  {
    return Page.EvalOnSelectorAllAsync(selector, expression, arg);
  }

  public Task<T> EvalOnSelectorAsync<T>(string selector, string expression, object? arg = null, PageEvalOnSelectorOptions? options = null)
  {
    return Page.EvalOnSelectorAsync<T>(selector, expression, arg, options);
  }

  public Task<JsonElement?> EvalOnSelectorAsync(string selector, string expression, object? arg = null)
  {
    return Page.EvalOnSelectorAsync(selector, expression, arg);
  }

  public Task<T> EvaluateAsync<T>(string expression, object? arg = null)
  {
    return Page.EvaluateAsync<T>(expression, arg);
  }

  public Task<JsonElement?> EvaluateAsync(string expression, object? arg = null)
  {
    return Page.EvaluateAsync(expression, arg);
  }

  public Task<IJSHandle> EvaluateHandleAsync(string expression, object? arg = null)
  {
    return Page.EvaluateHandleAsync(expression, arg);
  }

  public Task ExposeBindingAsync(string name, Action callback, PageExposeBindingOptions? options = null)
  {
    return Page.ExposeBindingAsync(name, callback, options);
  }

  public Task ExposeBindingAsync(string name, Action<BindingSource> callback)
  {
    return Page.ExposeBindingAsync(name, callback);
  }

  public Task ExposeBindingAsync<T>(string name, Action<BindingSource, T> callback)
  {
    return Page.ExposeBindingAsync(name, callback);
  }

  public Task ExposeBindingAsync<TResult>(string name, Func<BindingSource, TResult> callback)
  {
    return Page.ExposeBindingAsync(name, callback);
  }

  public Task ExposeBindingAsync<TResult>(string name, Func<BindingSource, IJSHandle, TResult> callback)
  {
    return Page.ExposeBindingAsync(name, callback);
  }

  public Task ExposeBindingAsync<T, TResult>(string name, Func<BindingSource, T, TResult> callback)
  {
    return Page.ExposeBindingAsync(name, callback);
  }

  public Task ExposeBindingAsync<T1, T2, TResult>(string name, Func<BindingSource, T1, T2, TResult> callback)
  {
    return Page.ExposeBindingAsync(name, callback);
  }

  public Task ExposeBindingAsync<T1, T2, T3, TResult>(string name, Func<BindingSource, T1, T2, T3, TResult> callback)
  {
    return Page.ExposeBindingAsync(name, callback);
  }

  public Task ExposeBindingAsync<T1, T2, T3, T4, TResult>(string name, Func<BindingSource, T1, T2, T3, T4, TResult> callback)
  {
    return Page.ExposeBindingAsync(name, callback);
  }

  public Task ExposeFunctionAsync(string name, Action callback)
  {
    return Page.ExposeFunctionAsync(name, callback);
  }

  public Task ExposeFunctionAsync<T>(string name, Action<T> callback)
  {
    return Page.ExposeFunctionAsync(name, callback);
  }

  public Task ExposeFunctionAsync<TResult>(string name, Func<TResult> callback)
  {
    return Page.ExposeFunctionAsync(name, callback);
  }

  public Task ExposeFunctionAsync<T, TResult>(string name, Func<T, TResult> callback)
  {
    return Page.ExposeFunctionAsync(name, callback);
  }

  public Task ExposeFunctionAsync<T1, T2, TResult>(string name, Func<T1, T2, TResult> callback)
  {
    return Page.ExposeFunctionAsync(name, callback);
  }

  public Task ExposeFunctionAsync<T1, T2, T3, TResult>(string name, Func<T1, T2, T3, TResult> callback)
  {
    return Page.ExposeFunctionAsync(name, callback);
  }

  public Task ExposeFunctionAsync<T1, T2, T3, T4, TResult>(string name, Func<T1, T2, T3, T4, TResult> callback)
  {
    return Page.ExposeFunctionAsync(name, callback);
  }

  public Task FillAsync(string selector, string value, PageFillOptions? options = null)
  {
    return Page.FillAsync(selector, value, options);
  }

  public Task FocusAsync(string selector, PageFocusOptions? options = null)
  {
    return Page.FocusAsync(selector, options);
  }

  public IFrame? Frame(string name)
  {
    return Page.Frame(name);
  }

  public IFrame? FrameByUrl(string url)
  {
    return Page.FrameByUrl(url);
  }

  public IFrame? FrameByUrl(Regex url)
  {
    return Page.FrameByUrl(url);
  }

  public IFrame? FrameByUrl(Func<string, bool> url)
  {
    return Page.FrameByUrl(url);
  }

  public IFrameLocator FrameLocator(string selector)
  {
    return Page.FrameLocator(selector);
  }

  public Task<string?> GetAttributeAsync(string selector, string name, PageGetAttributeOptions? options = null)
  {
    return Page.GetAttributeAsync(selector, name, options);
  }

  public ILocator GetByAltText(string text, PageGetByAltTextOptions? options = null)
  {
    return Page.GetByAltText(text, options);
  }

  public ILocator GetByAltText(Regex text, PageGetByAltTextOptions? options = null)
  {
    return Page.GetByAltText(text, options);
  }

  public ILocator GetByLabel(string text, PageGetByLabelOptions? options = null)
  {
    return Page.GetByLabel(text, options);
  }

  public ILocator GetByLabel(Regex text, PageGetByLabelOptions? options = null)
  {
    return Page.GetByLabel(text, options);
  }

  public ILocator GetByPlaceholder(string text, PageGetByPlaceholderOptions? options = null)
  {
    return Page.GetByPlaceholder(text, options);
  }

  public ILocator GetByPlaceholder(Regex text, PageGetByPlaceholderOptions? options = null)
  {
    return Page.GetByPlaceholder(text, options);
  }

  public ILocator GetByRole(AriaRole role, PageGetByRoleOptions? options = null)
  {
    return Page.GetByRole(role, options);
  }

  public ILocator GetByTestId(string testId)
  {
    return Page.GetByTestId(testId);
  }

  public ILocator GetByTestId(Regex testId)
  {
    return Page.GetByTestId(testId);
  }

  public ILocator GetByText(string text, PageGetByTextOptions? options = null)
  {
    return Page.GetByText(text, options);
  }

  public ILocator GetByText(Regex text, PageGetByTextOptions? options = null)
  {
    return Page.GetByText(text, options);
  }

  public ILocator GetByTitle(string text, PageGetByTitleOptions? options = null)
  {
    return Page.GetByTitle(text, options);
  }

  public ILocator GetByTitle(Regex text, PageGetByTitleOptions? options = null)
  {
    return Page.GetByTitle(text, options);
  }

  public Task<IResponse?> GoBackAsync(PageGoBackOptions? options = null)
  {
    return Page.GoBackAsync(options);
  }

  public Task<IResponse?> GoForwardAsync(PageGoForwardOptions? options = null)
  {
    return Page.GoForwardAsync(options);
  }

  public Task<IResponse?> GotoAsync(string url, PageGotoOptions? options = null)
  {
    return Page.GotoAsync(url, options);
  }

  public Task HoverAsync(string selector, PageHoverOptions? options = null)
  {
    return Page.HoverAsync(selector, options);
  }

  public Task<string> InnerHTMLAsync(string selector, PageInnerHTMLOptions? options = null)
  {
    return Page.InnerHTMLAsync(selector, options);
  }

  public Task<string> InnerTextAsync(string selector, PageInnerTextOptions? options = null)
  {
    return Page.InnerTextAsync(selector, options);
  }

  public Task<string> InputValueAsync(string selector, PageInputValueOptions? options = null)
  {
    return Page.InputValueAsync(selector, options);
  }

  public Task<bool> IsCheckedAsync(string selector, PageIsCheckedOptions? options = null)
  {
    return Page.IsCheckedAsync(selector, options);
  }

  public Task<bool> IsDisabledAsync(string selector, PageIsDisabledOptions? options = null)
  {
    return Page.IsDisabledAsync(selector, options);
  }

  public Task<bool> IsEditableAsync(string selector, PageIsEditableOptions? options = null)
  {
    return Page.IsEditableAsync(selector, options);
  }

  public Task<bool> IsEnabledAsync(string selector, PageIsEnabledOptions? options = null)
  {
    return Page.IsEnabledAsync(selector, options);
  }

  public Task<bool> IsHiddenAsync(string selector, PageIsHiddenOptions? options = null)
  {
    return Page.IsHiddenAsync(selector, options);
  }

  public Task<bool> IsVisibleAsync(string selector, PageIsVisibleOptions? options = null)
  {
    return Page.IsVisibleAsync(selector, options);
  }

  public ILocator Locator(string selector, PageLocatorOptions? options = null)
  {
    return Page.Locator(selector, options);
  }

  public Task<IPage?> OpenerAsync()
  {
    return Page.OpenerAsync();
  }

  public Task PauseAsync()
  {
    return Page.PauseAsync();
  }

  public Task<byte[]> PdfAsync(PagePdfOptions? options = null)
  {
    return Page.PdfAsync(options);
  }

  public Task PressAsync(string selector, string key, PagePressOptions? options = null)
  {
    return Page.PressAsync(selector, key, options);
  }

  public Task<IReadOnlyList<IElementHandle>> QuerySelectorAllAsync(string selector)
  {
    return Page.QuerySelectorAllAsync(selector);
  }

  public Task<IElementHandle?> QuerySelectorAsync(string selector, PageQuerySelectorOptions? options = null)
  {
    return Page.QuerySelectorAsync(selector, options);
  }

  public Task<IResponse?> ReloadAsync(PageReloadOptions? options = null)
  {
    return Page.ReloadAsync(options);
  }

  public Task RemoveLocatorHandlerAsync(ILocator locator)
  {
    return Page.RemoveLocatorHandlerAsync(locator);
  }

  public Task RouteAsync(string url, Action<IRoute> handler, PageRouteOptions? options = null)
  {
    return Page.RouteAsync(url, handler, options);
  }

  public Task RouteAsync(Regex url, Action<IRoute> handler, PageRouteOptions? options = null)
  {
    return Page.RouteAsync(url, handler, options);
  }

  public Task RouteAsync(Func<string, bool> url, Action<IRoute> handler, PageRouteOptions? options = null)
  {
    return Page.RouteAsync(url, handler, options);
  }

  public Task RouteAsync(string url, Func<IRoute, Task> handler, PageRouteOptions? options = null)
  {
    return Page.RouteAsync(url, handler, options);
  }

  public Task RouteAsync(Regex url, Func<IRoute, Task> handler, PageRouteOptions? options = null)
  {
    return Page.RouteAsync(url, handler, options);
  }

  public Task RouteAsync(Func<string, bool> url, Func<IRoute, Task> handler, PageRouteOptions? options = null)
  {
    return Page.RouteAsync(url, handler, options);
  }

  public Task RouteFromHARAsync(string har, PageRouteFromHAROptions? options = null)
  {
    return Page.RouteFromHARAsync(har, options);
  }

  public Task<IConsoleMessage> RunAndWaitForConsoleMessageAsync(Func<Task> action, PageRunAndWaitForConsoleMessageOptions? options = null)
  {
    return Page.RunAndWaitForConsoleMessageAsync(action, options);
  }

  public Task<IDownload> RunAndWaitForDownloadAsync(Func<Task> action, PageRunAndWaitForDownloadOptions? options = null)
  {
    return Page.RunAndWaitForDownloadAsync(action, options);
  }

  public Task<IFileChooser> RunAndWaitForFileChooserAsync(Func<Task> action, PageRunAndWaitForFileChooserOptions? options = null)
  {
    return Page.RunAndWaitForFileChooserAsync(action, options);
  }

  [Obsolete]
  public Task<IResponse?> RunAndWaitForNavigationAsync(Func<Task> action, PageRunAndWaitForNavigationOptions? options = null)
  {
    return Page.RunAndWaitForNavigationAsync(action, options);
  }

  public Task<IPage> RunAndWaitForPopupAsync(Func<Task> action, PageRunAndWaitForPopupOptions? options = null)
  {
    return Page.RunAndWaitForPopupAsync(action, options);
  }

  public Task<IRequest> RunAndWaitForRequestAsync(Func<Task> action, string urlOrPredicate, PageRunAndWaitForRequestOptions? options = null)
  {
    return Page.RunAndWaitForRequestAsync(action, urlOrPredicate, options);
  }

  public Task<IRequest> RunAndWaitForRequestAsync(Func<Task> action, Regex urlOrPredicate, PageRunAndWaitForRequestOptions? options = null)
  {
    return Page.RunAndWaitForRequestAsync(action, urlOrPredicate, options);
  }

  public Task<IRequest> RunAndWaitForRequestAsync(Func<Task> action, Func<IRequest, bool> urlOrPredicate, PageRunAndWaitForRequestOptions? options = null)
  {
    return Page.RunAndWaitForRequestAsync(action, urlOrPredicate, options);
  }

  public Task<IRequest> RunAndWaitForRequestFinishedAsync(Func<Task> action, PageRunAndWaitForRequestFinishedOptions? options = null)
  {
    return Page.RunAndWaitForRequestFinishedAsync(action, options);
  }

  public Task<IResponse> RunAndWaitForResponseAsync(Func<Task> action, string urlOrPredicate, PageRunAndWaitForResponseOptions? options = null)
  {
    return Page.RunAndWaitForResponseAsync(action, urlOrPredicate, options);
  }

  public Task<IResponse> RunAndWaitForResponseAsync(Func<Task> action, Regex urlOrPredicate, PageRunAndWaitForResponseOptions? options = null)
  {
    return Page.RunAndWaitForResponseAsync(action, urlOrPredicate, options);
  }

  public Task<IResponse> RunAndWaitForResponseAsync(Func<Task> action, Func<IResponse, bool> urlOrPredicate, PageRunAndWaitForResponseOptions? options = null)
  {
    return Page.RunAndWaitForResponseAsync(action, urlOrPredicate, options);
  }

  public Task<IWebSocket> RunAndWaitForWebSocketAsync(Func<Task> action, PageRunAndWaitForWebSocketOptions? options = null)
  {
    return Page.RunAndWaitForWebSocketAsync(action, options);
  }

  public Task<IWorker> RunAndWaitForWorkerAsync(Func<Task> action, PageRunAndWaitForWorkerOptions? options = null)
  {
    return Page.RunAndWaitForWorkerAsync(action, options);
  }

  public Task<byte[]> ScreenshotAsync(PageScreenshotOptions? options = null)
  {
    return Page.ScreenshotAsync(options);
  }

  public Task<IReadOnlyList<string>> SelectOptionAsync(string selector, string values, PageSelectOptionOptions? options = null)
  {
    return Page.SelectOptionAsync(selector, values, options);
  }

  public Task<IReadOnlyList<string>> SelectOptionAsync(string selector, IElementHandle values, PageSelectOptionOptions? options = null)
  {
    return Page.SelectOptionAsync(selector, values, options);
  }

  public Task<IReadOnlyList<string>> SelectOptionAsync(string selector, IEnumerable<string> values, PageSelectOptionOptions? options = null)
  {
    return Page.SelectOptionAsync(selector, values, options);
  }

  public Task<IReadOnlyList<string>> SelectOptionAsync(string selector, SelectOptionValue values, PageSelectOptionOptions? options = null)
  {
    return Page.SelectOptionAsync(selector, values, options);
  }

  public Task<IReadOnlyList<string>> SelectOptionAsync(string selector, IEnumerable<IElementHandle> values, PageSelectOptionOptions? options = null)
  {
    return Page.SelectOptionAsync(selector, values, options);
  }

  public Task<IReadOnlyList<string>> SelectOptionAsync(string selector, IEnumerable<SelectOptionValue> values, PageSelectOptionOptions? options = null)
  {
    return Page.SelectOptionAsync(selector, values, options);
  }

  public Task SetCheckedAsync(string selector, bool checkedState, PageSetCheckedOptions? options = null)
  {
    return Page.SetCheckedAsync(selector, checkedState, options);
  }

  public Task SetContentAsync(string html, PageSetContentOptions? options = null)
  {
    return Page.SetContentAsync(html, options);
  }

  public void SetDefaultNavigationTimeout(float timeout)
  {
    Page.SetDefaultNavigationTimeout(timeout);
  }

  public void SetDefaultTimeout(float timeout)
  {
    Page.SetDefaultTimeout(timeout);
  }

  public Task SetExtraHTTPHeadersAsync(IEnumerable<KeyValuePair<string, string>> headers)
  {
    return Page.SetExtraHTTPHeadersAsync(headers);
  }

  public Task SetInputFilesAsync(string selector, string files, PageSetInputFilesOptions? options = null)
  {
    return Page.SetInputFilesAsync(selector, files, options);
  }

  public Task SetInputFilesAsync(string selector, IEnumerable<string> files, PageSetInputFilesOptions? options = null)
  {
    return Page.SetInputFilesAsync(selector, files, options);
  }

  public Task SetInputFilesAsync(string selector, FilePayload files, PageSetInputFilesOptions? options = null)
  {
    return Page.SetInputFilesAsync(selector, files, options);
  }

  public Task SetInputFilesAsync(string selector, IEnumerable<FilePayload> files, PageSetInputFilesOptions? options = null)
  {
    return Page.SetInputFilesAsync(selector, files, options);
  }

  public Task SetViewportSizeAsync(int width, int height)
  {
    return Page.SetViewportSizeAsync(width, height);
  }

  public Task TapAsync(string selector, PageTapOptions? options = null)
  {
    return Page.TapAsync(selector, options);
  }

  public Task<string?> TextContentAsync(string selector, PageTextContentOptions? options = null)
  {
    return Page.TextContentAsync(selector, options);
  }

  public Task<string> TitleAsync()
  {
    return Page.TitleAsync();
  }

  [Obsolete]
  public Task TypeAsync(string selector, string text, PageTypeOptions? options = null)
  {
    return Page.TypeAsync(selector, text, options);
  }

  public Task UncheckAsync(string selector, PageUncheckOptions? options = null)
  {
    return Page.UncheckAsync(selector, options);
  }

  public Task UnrouteAllAsync(PageUnrouteAllOptions? options = null)
  {
    return Page.UnrouteAllAsync(options);
  }

  public Task UnrouteAsync(string url, Action<IRoute>? handler = null)
  {
    return Page.UnrouteAsync(url, handler);
  }

  public Task UnrouteAsync(Regex url, Action<IRoute>? handler = null)
  {
    return Page.UnrouteAsync(url, handler);
  }

  public Task UnrouteAsync(Func<string, bool> url, Action<IRoute>? handler = null)
  {
    return Page.UnrouteAsync(url, handler);
  }

  public Task UnrouteAsync(string url, Func<IRoute, Task> handler)
  {
    return Page.UnrouteAsync(url, handler);
  }

  public Task UnrouteAsync(Regex url, Func<IRoute, Task> handler)
  {
    return Page.UnrouteAsync(url, handler);
  }

  public Task UnrouteAsync(Func<string, bool> url, Func<IRoute, Task> handler)
  {
    return Page.UnrouteAsync(url, handler);
  }

  public Task<IConsoleMessage> WaitForConsoleMessageAsync(PageWaitForConsoleMessageOptions? options = null)
  {
    return Page.WaitForConsoleMessageAsync(options);
  }

  public Task<IDownload> WaitForDownloadAsync(PageWaitForDownloadOptions? options = null)
  {
    return Page.WaitForDownloadAsync(options);
  }

  public Task<IFileChooser> WaitForFileChooserAsync(PageWaitForFileChooserOptions? options = null)
  {
    return Page.WaitForFileChooserAsync(options);
  }

  public Task<IJSHandle> WaitForFunctionAsync(string expression, object? arg = null, PageWaitForFunctionOptions? options = null)
  {
    return Page.WaitForFunctionAsync(expression, arg, options);
  }

  public Task WaitForLoadStateAsync(LoadState? state = null, PageWaitForLoadStateOptions? options = null)
  {
    return Page.WaitForLoadStateAsync(state, options);
  }

  [Obsolete]
  public Task<IResponse?> WaitForNavigationAsync(PageWaitForNavigationOptions? options = null)
  {
    return Page.WaitForNavigationAsync(options);
  }

  public Task<IPage> WaitForPopupAsync(PageWaitForPopupOptions? options = null)
  {
    return Page.WaitForPopupAsync(options);
  }

  public Task<IRequest> WaitForRequestAsync(string urlOrPredicate, PageWaitForRequestOptions? options = null)
  {
    return Page.WaitForRequestAsync(urlOrPredicate, options);
  }

  public Task<IRequest> WaitForRequestAsync(Regex urlOrPredicate, PageWaitForRequestOptions? options = null)
  {
    return Page.WaitForRequestAsync(urlOrPredicate, options);
  }

  public Task<IRequest> WaitForRequestAsync(Func<IRequest, bool> urlOrPredicate, PageWaitForRequestOptions? options = null)
  {
    return Page.WaitForRequestAsync(urlOrPredicate, options);
  }

  public Task<IRequest> WaitForRequestFinishedAsync(PageWaitForRequestFinishedOptions? options = null)
  {
    return Page.WaitForRequestFinishedAsync(options);
  }

  public Task<IResponse> WaitForResponseAsync(string urlOrPredicate, PageWaitForResponseOptions? options = null)
  {
    return Page.WaitForResponseAsync(urlOrPredicate, options);
  }

  public Task<IResponse> WaitForResponseAsync(Regex urlOrPredicate, PageWaitForResponseOptions? options = null)
  {
    return Page.WaitForResponseAsync(urlOrPredicate, options);
  }

  public Task<IResponse> WaitForResponseAsync(Func<IResponse, bool> urlOrPredicate, PageWaitForResponseOptions? options = null)
  {
    return Page.WaitForResponseAsync(urlOrPredicate, options);
  }

  public Task<IElementHandle?> WaitForSelectorAsync(string selector, PageWaitForSelectorOptions? options = null)
  {
    return Page.WaitForSelectorAsync(selector, options);
  }

  public Task WaitForTimeoutAsync(float timeout)
  {
    return Page.WaitForTimeoutAsync(timeout);
  }

  public Task WaitForURLAsync(string url, PageWaitForURLOptions? options = null)
  {
    return Page.WaitForURLAsync(url, options);
  }

  public Task WaitForURLAsync(Regex url, PageWaitForURLOptions? options = null)
  {
    return Page.WaitForURLAsync(url, options);
  }

  public Task WaitForURLAsync(Func<string, bool> url, PageWaitForURLOptions? options = null)
  {
    return Page.WaitForURLAsync(url, options);
  }

  public Task<IWebSocket> WaitForWebSocketAsync(PageWaitForWebSocketOptions? options = null)
  {
    return Page.WaitForWebSocketAsync(options);
  }

  public Task<IWorker> WaitForWorkerAsync(PageWaitForWorkerOptions? options = null)
  {
    return Page.WaitForWorkerAsync(options);
  }
}