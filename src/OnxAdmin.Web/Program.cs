#pragma warning disable SKEXP0001, SKEXP0020

using System.IO.Abstractions;

using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddLogging(c => 
  c.AddJsonConsole(o => 
  {
    o.IncludeScopes = true;
    o.TimestampFormat = "HH:mm:ss ";
    o.UseUtcTimestamp = true;
    o.JsonWriterOptions = new() { Indented = true };
  })
  .SetMinimumLevel(LogLevel.Information)
);
builder.Services.AddHttpLogging(o => o.LoggingFields = HttpLoggingFields.All);

builder.Services.ConfigureOptions<AnthropicOptionsSetup>();
builder.Services.ConfigureHttpClientDefaults(
  c => c.AddStandardResilienceHandler().Configure(o =>
  {
    // Ideally wouldn't need to do this, but since tool calling
    // not streamed we need to increase the timeout for the
    // long running calls.
    o.AttemptTimeout = new() { Timeout = TimeSpan.FromMinutes(5) };
    o.TotalRequestTimeout = new() { Timeout = TimeSpan.FromMinutes(15) };
    o.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(10);
    o.Retry.ShouldHandle = args => ValueTask.FromResult(args.Outcome.Result?.StatusCode is HttpStatusCode.Unauthorized);
  })
);
builder.Services.AddHttpClient<IAnthropicService, AnthropicService>();

builder.Services.ConfigureOptions<OnspringOptionsSetup>();
builder.Services.AddScoped<IOnspringService, OnspringService>();

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

builder.Services.AddScoped<IFileSystem, FileSystem>();

builder.Services.AddScoped<IWeatherService, WeatherService>();

var generateEmbeddings = builder.Configuration.GetValue("GenerateEmbeddings", false);

if (generateEmbeddings)
{
  builder.Services.AddHostedService<HelpCenterGenerateEmbeddingsService>();
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();