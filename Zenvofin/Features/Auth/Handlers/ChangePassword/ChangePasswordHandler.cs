using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.ChangePassword;

public sealed class ChangePasswordHandler(
    UserManager<User> userManager,
    IMemoryCache cache,
    ILogger<ChangePasswordHandler> logger)
{
    public async Task<Result> Handle(ChangePasswordCommand command)
    {
        try
        {
            User? user = await userManager.FindByIdAsync(command.UserId.ToString());

            if (user is null)
            {
                logger.LogWarning("User not found with ID {UserId}.", command.UserId);
                return Result.Fail("User not found.", ResultCode.NotFound);
            }

            IdentityResult result = await userManager.ChangePasswordAsync(
                user,
                command.CurrentPassword,
                command.NewPassword);

            if (!result.Succeeded)
            {
                List<string> errors = result.Errors.Select(e => e.Description).ToList();
                logger.LogInformation(
                    "Failed to change password for user {UserId}. Errors: {Errors}",
                    command.UserId,
                    string.Join(", ", errors));
                return Result.Fail(errors);
            }

            cache.Remove(AuthHelpers.UserInfoKey(command.UserId));

            logger.LogInformation(
                "Password for user {UserId} has been changed successfully.",
                command.UserId);

            return Result.Success("Password changed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while changing password for user {UserId}.",
                command.UserId);
            return Result.Fail(ErrorMessage.Exception, ResultCode.InternalError);
        }
    }
}