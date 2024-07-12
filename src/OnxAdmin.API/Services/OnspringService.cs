namespace OnxAdmin.API.Services;

interface IOnspringService
{
  Task<List<HelpCenterDocument>> GetHelpCenterDocumentsAsync();
}

class OnspringService : IOnspringService
{
  public Task<List<HelpCenterDocument>> GetHelpCenterDocumentsAsync()
  {
    throw new NotImplementedException();
  }
}