using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.AccessToken;

public sealed class AccessTokenHandler(IConfiguration configuration)
{
    public Result<string> Handle(AccessTokenCommand accessTokenCommandEvent)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, accessTokenCommandEvent.UserId.ToString()),
            new(ClaimTypes.Thumbprint, accessTokenCommandEvent.DeviceId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        ];

        string jwtSecret = Environment.GetEnvironmentVariable(ConfigurationConstants.JwtSecretEnvironment)
                           ?? throw new InvalidOperationException("Secret not found");

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtSecret));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            configuration[ConfigurationConstants.JwtIssuer],
            configuration[ConfigurationConstants.JwtAudience],
            claims,
            expires: DateTime.UtcNow.AddMinutes(AccessTokenConstants.ExpirationTimeInMinutes),
            signingCredentials: credentials);

        string jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Result<string>.Success(jwt, "Token generated successfully");
    }
}