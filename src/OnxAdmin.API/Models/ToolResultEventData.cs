namespace OnxAdmin.API.Models;

class ToolResultEventData(ToolResultContent toolResultContent) : EventData("tool_result")
{
  public Message Message { get; } = new Message(MessageRole.User, [toolResultContent]);
}