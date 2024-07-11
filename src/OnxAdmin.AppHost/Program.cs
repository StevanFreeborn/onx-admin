var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.OnxAdmin_API>("api")
  .WithExternalHttpEndpoints();

builder.AddNpmApp("client", "../OnxAdmin.Client")
  .WithReference(api)
  .WithHttpsEndpoint(port: 4000, env: "PORT")
  .WithExternalHttpEndpoints()
  .PublishAsDockerFile();

builder.Build().Run();
