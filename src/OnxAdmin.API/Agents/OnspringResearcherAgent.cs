#pragma warning disable SKEXP0001

namespace OnxAdmin.API.Agents;

interface IOnspringResearcherAgent
{
  Task<List<Finding>> ExecuteTaskAsync(string input);
}

class OnspringResearcherAgent(
  ISemanticTextMemory memory,
  Instrumentation instrumentation
) : IOnspringResearcherAgent
{
  private readonly ISemanticTextMemory _memory = memory;
  private readonly ActivitySource _activitySource = instrumentation.ActivitySource;

  public async Task<List<Finding>> ExecuteTaskAsync(string input)
  {
    using var activity = _activitySource.StartActivity(nameof(ExecuteTaskAsync));
    activity?.SetTag("input", input);

    var results = _memory.SearchAsync(
      collection: HelpCenterGenerateEmbeddingsService.HelpCenterCollection,
      query: $"search_query: {input}",
      limit: 10,
      minRelevanceScore: 0
    );


    var findings = new List<Finding>();
    var memoryResults = new List<MemoryQueryResult>();

    await foreach (var result in results)
    {
      memoryResults.Add(result);
      var source = result.Metadata.AdditionalMetadata;
      var content = result.Metadata.Text.Replace("search_document: ", string.Empty);
      findings.Add(new Finding(content, source));
    }

    activity?.SetTag("output", JsonSerializer.Serialize(memoryResults, JSON.Options));

    return findings;
  }
}

record Finding
{
  public string Content { get; init; }
  public string Source { get; init; }

  public Finding() : this(string.Empty, string.Empty) { }

  public Finding(string content, string source)
  {
    Content = content;
    Source = source;
  }
}