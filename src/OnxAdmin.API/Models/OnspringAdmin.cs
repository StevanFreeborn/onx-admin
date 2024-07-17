using OnxAdmin.API.Factories;

namespace OnxAdmin.API.Models;

interface IOnspringAdmin
{
  Task<T> PerformActionAsync<T>(Func<IOnspringPage, Task<T>> action);
}

class OnspringAdmin(IPageFactory pageFactory) : IOnspringAdmin
{
  private readonly IPageFactory _pageFactory = pageFactory;

  public async Task<T> PerformActionAsync<T>(Func<IOnspringPage, Task<T>> action)
  {
    var page = await _pageFactory.CreatePageAsync();

    try
    {
      return await action(page);
    }
    finally
    {
      await page.CloseAsync();
    }
  }
}