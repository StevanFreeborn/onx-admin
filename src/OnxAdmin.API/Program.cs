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
  o.SerializerOptions.PropertyNameCaseInsensitive = true;
  o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddSingleton<Instrumentation>();
builder.Services
  .AddOpenTelemetry()
  .WithTracing(tracing => tracing.AddSource(Instrumentation.ActivitySourceName));

builder.Services.AddOnspring();
builder.Services.AddAnthropic();
builder.Services.AddMemory();
builder.Services.AddAttachmentService();
builder.Services.AddAgents();

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

app.MapPost("/generate-response", async ([AsParameters] GenerateResponseRequest request) =>
{
  var events = request.ChatService.GenerateResponseAsync(request.GenerateResponseDto.Conversation);

  request.Context.Response.Headers.Append("Content-Type", "text/event-stream");

  await foreach (var e in events)
  {
    await request.Context.Response.WriteAsync($"{e.ToJson()}\n");
    await request.Context.Response.Body.FlushAsync();
    await Task.Delay(100);
  }
});

app
  .MapPost("/attachments", async ([AsParameters] AddAttachmentRequest request) =>
  {
    if (request.Attachment is null)
    {
      return Results.ValidationProblem(new Dictionary<string, string[]>
      {
        { nameof(request.Attachment), ["Attachment is required."] }
      });
    }

    var id = await request.AttachmentService.AddAttachmentAsync(request.Attachment);
    return Results.Ok(new { id });
  })
  .DisableAntiforgery();

app.MapDelete("/attachments/{AttachmentId}", async ([AsParameters] DeleteAttachmentRequest request) =>
{
  var result = await request.AttachmentService.RemoveAttachmentAsync(request.AttachmentId);
  return result ? Results.NoContent() : Results.Problem(statusCode: 500, detail: "Failed to remove attachment.");
});

app.UseHttpsRedirection();
app.UseCors(static builder =>
  builder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin()
);

app.Run();