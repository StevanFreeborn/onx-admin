
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddProblemDetails();
builder.Services.ConfigureHttpJsonOptions(o =>
{
  o.SerializerOptions.Converters.Add(new ContentConverter());
  o.SerializerOptions.Converters.Add(new EventDataConverter());
  o.SerializerOptions.Converters.Add(new DeltaConverter());
});

builder.Services.AddSingleton<Instrumentation>();
builder.Services
  .AddOpenTelemetry()
  .WithTracing(tracing => tracing.AddSource(Instrumentation.ActivitySourceName));

builder.Services.AddAgents();
builder.Services.AddAnthropic();
builder.Services.AddMemory();

if (builder.Configuration.GetValue("GenerateEmbeddings", false))
{
  builder.Services.AddHostedService<HelpCenterGenerateEmbeddingsService>();
}

var app = builder.Build();

app.UseStatusCodePages();

app.UseMiddleware<ErrorMiddleware>();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.MapPost("/generate-response", ([AsParameters] GenerateResponseRequest request) =>
{
  return request.ChatService.GenerateResponseAsync(request.GenerateResponseDto.Conversation);
});

app.UseHttpsRedirection();
app.UseCors(static builder =>
  builder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin()
);

app.Run();