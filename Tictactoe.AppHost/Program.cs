var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.Tictactoe_Server>("tictactoe-server");

var front = builder.AddNpmApp("tictactoe-client", "../tictactoe-client", "dev")
    .WithReference(server)
    .WaitFor(server)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

server.WithEnvironment(ctx =>
{
    ctx.EnvironmentVariables["AllowedOrigins"] = front.Resource.GetEndpoint("http");
});

builder.Build().Run();
