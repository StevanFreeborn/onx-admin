namespace OnxAdmin.API.Extensions;

static class EventDataExtensions
{
  public static string ToJson(this EventData eventData) => JsonSerializer.Serialize(eventData, JSON.Options);
}