using Microsoft.EntityFrameworkCore;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.LogoutUser;

public sealed class LogoutUserHandler(AuthDbContext context, ILogger<LogoutUserHandler> logger)
{
    public async Task<Result> Handle(LogoutUserCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            Data.RefreshToken? refreshToken = await context.RefreshTokens
                .Where(rt => rt.UserId == request.UserId && rt.DeviceId == request.DeviceId && !rt.IsRevoked)
                .FirstOrDefaultAsync(cancellationToken);

            if (refreshToken is null)
            {
                logger.LogWarning(
                    "No active refresh token found for user {UserId} with device {DeviceId}.",
                    request.UserId,
                    request.DeviceId);
                return Result.Fail("No active session found for this device.", ResultCode.NotFound);
            }

            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "User {UserId} has been logged out from device {DeviceId}.",
                request.UserId,
                request.DeviceId);

            return Result.Success("User logged out successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while logging out user {UserId} from device {DeviceId}.",
                request.UserId,
                request.DeviceId);
            return Result.Fail(ErrorMessage.Exception, ResultCode.InternalError);
        }
    }
}