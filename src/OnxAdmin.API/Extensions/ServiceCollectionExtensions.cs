#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0020

namespace OnxAdmin.API.Extensions;

static class ServiceCollectionExtensions
{
  public static IServiceCollection AddAgents(this IServiceCollection services)
  {
    services.AddScoped<ITopicSentryAgent, TopicSentryAgent>();
    services.AddScoped<IOnspringResearcherAgent, OnspringResearcherAgent>();
    services.AddOnspringAdministratorAgent();
    return services;
  }

  public static IServiceCollection AddOnspring(this IServiceCollection services)
  {
    services.ConfigureOptions<OnspringOptionsSetup>();
    services.AddScoped<IPageFactory, PageFactory>();
    services.AddScoped<IOnspringAdmin, OnspringAdmin>();
    services.AddScoped<IOnspringService, OnspringService>();
    return services;
  }

  public static IServiceCollection AddAnthropic(this IServiceCollection services)
  {
    services.ConfigureOptions<AnthropicOptionsSetup>();
    services.AddTransient<IAnthropicApiClient, AnthropicApiClient>(sp =>
    {
      var options = sp.GetRequiredService<IOptions<AnthropicOptions>>().Value;
      var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
      var httpClient = httpClientFactory.CreateClient();
      return new AnthropicApiClient(options.Key, httpClient);
    });
    services.AddScoped<IChatService, AnthropicChatService>();
    return services;
  }

  public static IServiceCollection AddMemory(this IServiceCollection services)
  {
    services.ConfigureOptions<ChromaOptionsSetup>();
    services.AddTransient<IMemoryStore>(sp =>
    {
      var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
      var options = sp.GetRequiredService<IOptions<ChromaOptions>>().Value;
      var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
      return new ChromaMemoryStore(httpClient, options.BaseUrl, loggerFactory);
    });

    services.ConfigureOptions<OllamaTextEmbeddingOptionsSetup>();

    services.AddTransient<ITextEmbeddingGenerationService, OllamaTextEmbeddingGeneration>(sp =>
    {
      var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
      var options = sp.GetRequiredService<IOptions<OllamaTextEmbeddingOptions>>().Value;
      var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
      return new OllamaTextEmbeddingGeneration(options.ModelId, options.BaseUrl, httpClient, loggerFactory);
    });

    services.AddTransient<ISemanticTextMemory, SemanticTextMemory>();
    return services;
  }

  public static IServiceCollection AddAttachmentService(this IServiceCollection services)
  {
    services.AddScoped<IFileSystem, FileSystem>();
    services.AddScoped<IAttachmentService, FileSystemAttachmentService>();
    return services;
  }
}