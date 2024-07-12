var builder = DistributedApplication.CreateBuilder(args);

var chroma = builder.AddContainer("chroma", "chromadb/chroma")
  .WithVolume("onx_admin_chroma_data", "/chroma/chroma")
  .WithHttpEndpoint(targetPort: 8000)
  .WithExternalHttpEndpoints()
  .PublishAsContainer();

var ollama = builder.AddContainer("ollama", "ollama/ollama")
  .WithVolume("onx_admin_ollama_data", "/root/.ollama")
  .WithBindMount("../OnxAdmin.Containers/Ollama/entrypoint.sh", "/entrypoint.sh")
  .WithHttpEndpoint(targetPort: 11434)
  .WithExternalHttpEndpoints()
  .WithEntrypoint("/entrypoint.sh")
  .PublishAsContainer();

var api = builder.AddProject<Projects.OnxAdmin_API>("api")
  .WithEnvironment("ChromaOptions__BaseUrl", chroma.GetEndpoint("http"))
  .WithEnvironment("OllamaTextEmbeddingOptions__BaseUrl", ollama.GetEndpoint("http"))
  .WithEnvironment("OllamaTextEmbeddingOptions__ModelId", "nomic-embed-text")
  .WithExternalHttpEndpoints();

builder.AddNpmApp("client", "../OnxAdmin.Client")
  .WithReference(api)
  .WithHttpsEndpoint(port: 4000, env: "PORT")
  .WithExternalHttpEndpoints()
  .PublishAsDockerFile();

builder.Build().Run();