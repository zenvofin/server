using Microsoft.EntityFrameworkCore;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Features.Auth.Handlers.RefreshToken;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.ValidateRefreshToken;

public sealed class ValidateRefreshTokenHandler(AuthDbContext context, ILogger<ValidateRefreshTokenHandler> logger)
{
    public async Task<Result<RefreshTokenCommand>> Handle(
        ValidateRefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            string hashedToken = AuthHelpers.HashToken(command.RefreshToken);

            Data.RefreshToken? refreshToken = await context.RefreshTokens
                .Where(rt => rt.UserId == command.UserId
                             && rt.DeviceId == command.DeviceId
                             && rt.Token == hashedToken
                             && !rt.IsRevoked)
                .FirstOrDefaultAsync(cancellationToken);

            if (refreshToken is null)
            {
                logger.LogWarning(
                    "Invalid or revoked refresh token for user {UserId} with device {DeviceId}.",
                    command.UserId,
                    command.DeviceId);
                return Result<RefreshTokenCommand>.Fail("Invalid or revoked refresh token.", ResultCode.Unauthorized);
            }

            if (refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                logger.LogInformation(
                    "Expired refresh token for user {UserId} with device {DeviceId}.",
                    command.UserId,
                    command.DeviceId);
                return Result<RefreshTokenCommand>.Fail("Refresh token has expired.", ResultCode.Unauthorized);
            }

            logger.LogInformation(
                "Refresh token validated successfully for user {UserId} with device {DeviceId}.",
                command.UserId,
                command.DeviceId);

            RefreshTokenCommand refreshTokenCommandEvent = new(command.UserId, command.DeviceId);

            return Result<RefreshTokenCommand>.Success(refreshTokenCommandEvent);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while validating refresh token for user {UserId}.",
                command.UserId);
            return Result<RefreshTokenCommand>.Fail(ErrorMessage.Exception, ResultCode.InternalError);
        }
    }
}