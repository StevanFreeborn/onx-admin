var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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
