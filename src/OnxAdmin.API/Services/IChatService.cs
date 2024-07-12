namespace OnxAdmin.API.Services;

interface IChatService
{
  Task GenerateResponseAsync(List<Message> messages);
}