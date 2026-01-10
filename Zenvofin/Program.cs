using DotNetEnv;
using FastEndpoints;
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
        .WriteTo.Seq(context.Configuration["SerilogUrl"]!, apiKey: Environment.GetEnvironmentVariable("SEQ_API_KEY")!);
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFastEndpoints();

builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddDbServices();

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
    options.Endpoints.RoutePrefix = "api/mobile";
    options.Versioning.Prefix = "v";
    options.Endpoints.Configurator = ep => { ep.PreProcessor<CorrelationPreProcessor>(Order.Before); };
});

await app.RunAsync();