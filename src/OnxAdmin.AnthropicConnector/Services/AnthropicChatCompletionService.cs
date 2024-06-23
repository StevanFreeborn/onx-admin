
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace OnxAdmin.AnthropicConnector.Services;

public class AnthropicChatCompletionService : IChatCompletionService
{
  public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

  public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
    ChatHistory chatHistory,
    PromptExecutionSettings? executionSettings = null,
    Kernel? kernel = null,
    CancellationToken cancellationToken = default
  )
  {
    throw new NotImplementedException();
  }

  public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
    ChatHistory chatHistory,
    PromptExecutionSettings? executionSettings = null,
    Kernel? kernel = null,
    CancellationToken cancellationToken = default
  )
  {
    throw new NotImplementedException();
  }
}