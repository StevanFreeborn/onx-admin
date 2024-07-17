namespace OnxAdmin.API.Models;

record HelpCenterDocument
{
  public string Title { get; init; }
  public string Path { get; init; }
  public string Content { get; init; }

  public HelpCenterDocument(string title, string path, string content)
  {
    Title = title;
    Path = path;
    Content = content;
  }
}