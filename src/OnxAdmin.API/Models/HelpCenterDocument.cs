namespace OnxAdmin.API.Models;

record HelpCenterDocument
{
  public string Title { get; init; }
  public string Content { get; init; }

  public HelpCenterDocument(string title, string content)
  {
    Title = title;
    Content = content;
  }
}