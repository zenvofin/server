using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Zenvofin.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Env.Load("../.env");

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(options =>
    {
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION")));

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

await app.RunAsync();