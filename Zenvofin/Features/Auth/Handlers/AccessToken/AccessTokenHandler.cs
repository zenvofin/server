using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.AccessToken;

public sealed class AccessTokenHandler(IConfiguration configuration)
{
    public Result<AccessTokenResponse> Handle(AccessTokenCommand command)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, command.UserId.ToString()),
            new(ClaimTypes.Thumbprint, command.DeviceId.ToString()),
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

        AccessTokenResponse tokens = new(jwt, command.RefreshToken);

        return Result<AccessTokenResponse>.Success(tokens, "Token generated successfully");
    }
}