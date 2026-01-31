using DotNetEnv;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Wolverine;
using Zenvofin.Extensions;
using Zenvofin.Shared;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Env.Load("../.env");

builder.Host.UseWolverine();
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console();
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFastEndpoints();

builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddDbServices(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Zenvofin API";
        options.Theme = ScalarTheme.Kepler;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();
app.UseFastEndpoints(options =>
{
    options.Endpoints.RoutePrefix = "api";
    options.Versioning.Prefix = "v";
    options.Endpoints.Configurator = ep =>
    {
        ep.PreProcessor<CorrelationPreProcessor>(Order.Before);
        ep.AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
    };
});

await app.RunAsync();