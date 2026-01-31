using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresDatabaseResource> postgres = builder.AddPostgres("postgres")
    .WithImage("khavol/zenvofin-postgres:latest")
    .WithVolume("zenvofin-postgres", "/var/lib/postgresql")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithArgs("-c", "shared_preload_libraries=pg_cron", "-c", "cron.database_name=zenvofin")
    .AddDatabase("zenvofin");

builder.AddProject<Zenvofin>("zenvofin-api")
    .WithReference(postgres);

builder.Build().Run();