namespace OnxAdmin.API.Services;

interface IChatService
{
  IAsyncEnumerable<EventData> GenerateResponseAsync(List<Message> messages);
}