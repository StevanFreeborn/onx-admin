var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.ConfigureOptions<AnthropicOptionsSetup>();
builder.Services.ConfigureHttpClientDefaults(c => c.AddStandardResilienceHandler());
builder.Services.AddHttpClient<IAnthropicService, AnthropicService>();

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
