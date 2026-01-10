using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Features.Auth.Handlers.AccessToken;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.RefreshToken;

public sealed class RefreshTokenHandler(AuthDbContext context, ILogger<RefreshTokenHandler> logger)
{
    public async Task<Result<AccessTokenCommand>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await RevokeExistingToken(command, cancellationToken);

            byte[] tokenBytes = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }

            string tokenString = Convert.ToBase64String(tokenBytes);

            Data.RefreshToken refreshToken = new()
            {
                UserId = command.UserId,
                DeviceId = command.DeviceId,
                Token = HashToken(tokenString),
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenConstants.ExpirationTimeInDays),
            };

            context.RefreshTokens.Add(refreshToken);

            logger.LogInformation(
                "Token for user {UserId} with device {DeviceId} has been refreshed.",
                command.UserId,
                command.DeviceId);

            await context.SaveChangesAsync(cancellationToken);

            AccessTokenCommand tokenResponse = new(command.UserId, command.DeviceId, tokenString);

            return Result<AccessTokenCommand>.Success(tokenResponse);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while generating a refresh token for user {UserId}.",
                command.UserId);
            return Result<AccessTokenCommand>.Fail(ErrorMessage.Exception, ResultCode.InternalError);
        }
    }

    private static string HashToken(string token)
    {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }

    private async Task RevokeExistingToken(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        Data.RefreshToken? existingToken = await context.RefreshTokens
            .Where(rt => rt.DeviceId == command.DeviceId && !rt.IsRevoked)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingToken is not null)
        {
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;

            logger.LogInformation(
                "Token for user {UserId} with device {DeviceId} has been revoked.",
                command.UserId,
                command.DeviceId);
        }
    }
}