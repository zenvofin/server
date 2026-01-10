using Microsoft.EntityFrameworkCore;
using Zenvofin.Features.Auth.Data;

namespace Zenvofin.Extensions;

public static class DbContextServiceCollectionExtensions
{
    public static IServiceCollection AddDbServices(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION")));

        return services;
    }
}