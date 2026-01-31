using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Zenvofin>("zenvofin");

builder.Build().Run();