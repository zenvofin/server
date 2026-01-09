using DotNetEnv;
using FastEndpoints;
using Scalar.AspNetCore;
using Wolverine;
using Zenvofin.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Env.Load("../.env");

builder.Host.UseWolverine();

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

app.UseFastEndpoints();

await app.RunAsync();