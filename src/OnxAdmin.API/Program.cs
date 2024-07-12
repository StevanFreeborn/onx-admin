#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0020

using Codeblaze.SemanticKernel.Connectors.Ollama;

using Microsoft.SemanticKernel.Connectors.Chroma;
using Microsoft.SemanticKernel.Embeddings;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddTransient<ITopicSentryAgent, TopicSentryAgent>();

builder.Services.ConfigureOptions<AnthropicOptionsSetup>();
builder.Services.AddTransient<IAnthropicApiClient, AnthropicApiClient>(sp =>
{
  var options = sp.GetRequiredService<IOptions<AnthropicOptions>>().Value;
  var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
  var httpClient = httpClientFactory.CreateClient();
  return new AnthropicApiClient(options.Key, httpClient);
});
builder.Services.AddScoped<IChatService, AnthropicChatService>();

builder.Services.ConfigureOptions<ChromaOptionsSetup>();
builder.Services.AddTransient<IMemoryStore>(sp =>
{
  var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
  var options = sp.GetRequiredService<IOptions<ChromaOptions>>().Value;
  var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
  return new ChromaMemoryStore(httpClient, options.BaseUrl, loggerFactory);
});
builder.Services.ConfigureOptions<OllamaTextEmbeddingOptionsSetup>();
builder.Services.AddTransient<ITextEmbeddingGenerationService, OllamaTextEmbeddingGeneration>(sp =>
{
  var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
  var options = sp.GetRequiredService<IOptions<OllamaTextEmbeddingOptions>>().Value;
  var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
  return new OllamaTextEmbeddingGeneration(options.ModelId, options.BaseUrl, httpClient, loggerFactory);
});
builder.Services.AddTransient<ISemanticTextMemory, SemanticTextMemory>();

builder.Services.ConfigureOptions<OnspringOptionsSetup>();
builder.Services.AddScoped<IOnspringService, OnspringService>();

var generateEmbeddings = builder.Configuration.GetValue("GenerateEmbeddings", false);

if (generateEmbeddings)
{
  builder.Services.AddHostedService<HelpCenterGenerateEmbeddingsService>();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapPost("/generate-response", ([AsParameters] GenerateResponseRequest request) =>
{
  return Results.Ok(new { message = "Hello, World!" });
});

app.UseHttpsRedirection();
app.UseCors(static builder =>
  builder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin()
);

app.Run();