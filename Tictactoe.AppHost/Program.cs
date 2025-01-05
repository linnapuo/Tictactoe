var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddProject<Projects.Tictactoe_Server>("tictactoe-server");

builder.AddNpmApp("tictactoe-client", "../tictactoe-client", "dev")
    .WithReference(server)
    .WithEnvironment("BROWSER", "none")
    .WithHttpEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.AddProject<Projects.Tictactoe_Authentication>("tictactoe-authentication");

builder.Build().Run();
