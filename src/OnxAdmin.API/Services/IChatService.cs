namespace OnxAdmin.API.Services;

interface IChatService
{
  Task<Message> GenerateResponseAsync(List<Message> messages);
}