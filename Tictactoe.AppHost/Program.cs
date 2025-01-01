var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Tictactoe_Server>("tictactoe-server");

builder.Build().Run();
