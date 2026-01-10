using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Shared;

namespace Zenvofin.Extensions;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ";
            })
            .AddEntityFrameworkStores<AuthDbContext>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration[ConfigurationConstants.JwtIssuer],
                    ValidAudience = configuration[ConfigurationConstants.JwtAudience],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            Environment.GetEnvironmentVariable(ConfigurationConstants.JwtSecretEnvironment)
                            ?? throw new InvalidOperationException("Secret not configured"))),
                };
            });

        services.AddAuthorization();

        return services;
    }
}