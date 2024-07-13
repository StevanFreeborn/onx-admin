#pragma warning disable SKEXP0001, SKEXP0050

namespace OnxAdmin.API.Services;

class HelpCenterGenerateEmbeddingsService(
  IServiceProvider serviceProvider,
  ILogger<HelpCenterGenerateEmbeddingsService> logger
) : IHostedService
{
  public static string HelpCenterCollection = "help_center_documents";
  private readonly IServiceProvider _serviceProvider = serviceProvider;
  private readonly ILogger<HelpCenterGenerateEmbeddingsService> _logger = logger;

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await using var scope = _serviceProvider.CreateAsyncScope();
    var onspringService = scope.ServiceProvider.GetRequiredService<IOnspringService>();
    var memory = scope.ServiceProvider.GetRequiredService<ISemanticTextMemory>();

    _logger.LogInformation("Generating embeddings for documents in the help center");

    var documents = await onspringService.GetHelpCenterDocumentsAsync();

    foreach (var document in documents)
    {
      var lines = TextChunker.SplitPlainTextLines(document.Content, 40);
      var chunks = TextChunker.SplitPlainTextParagraphs(lines, 120);

      foreach (var (chunk, index) in chunks.Select((chunk, index) => (chunk, index)))
      {
        try
        {
          await memory.SaveInformationAsync(
            collection: HelpCenterCollection,
            id: $"{document.Title}_{index}",
            description: document.Title,
            text: $"search_document: {chunk}",
            additionalMetadata: document.Path,
            cancellationToken: cancellationToken
          );
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error generating embeddings for {Title} {Index}", document.Title, index);
          continue;
        }
      }

      _logger.LogInformation("Embeddings generated for {Title}", document.Title);
    };

    _logger.LogInformation("Embeddings generated for all documents in the help center");
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}