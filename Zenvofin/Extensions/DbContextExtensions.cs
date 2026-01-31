using Microsoft.EntityFrameworkCore;
using Zenvofin.Features.Auth.Data;

namespace Zenvofin.Extensions;

public static class DbContextServiceCollectionExtensions
{
    public static IServiceCollection AddDbServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("zenvofin")));

        return services;
    }
}