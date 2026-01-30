using Microsoft.AspNetCore.Identity;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.UpdateUserInformation;

public sealed class UpdateUserInformationHandler(
    UserManager<User> userManager,
    ILogger<UpdateUserInformationHandler> logger)
{
    public async Task<Result> Handle(UpdateUserInformationCommand command)
    {
        try
        {
            User? user = await userManager.FindByIdAsync(command.UserId.ToString());

            if (user is null)
            {
                logger.LogWarning(
                    "User not found with ID {UserId}.",
                    command.UserId);
                return Result.Fail("User not found.", ResultCode.NotFound);
            }

            bool hasChanges = false;

            if (user.Email != command.Email)
            {
                user.Email = command.Email;
                hasChanges = true;
            }

            if (user.UserName != command.UserName)
            {
                user.UserName = command.UserName;
                hasChanges = true;
            }

            if (hasChanges)
            {
                IdentityResult result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    List<string> errors = result.Errors.Select(e => e.Description).ToList();
                    logger.LogWarning(
                        "Failed to update user {UserId}. Errors: {Errors}",
                        command.UserId,
                        string.Join(", ", errors));
                    return Result.Fail(errors);
                }

                logger.LogInformation(
                    "User {UserId} information has been updated successfully.",
                    command.UserId);
            }

            return Result.Success("User information updated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while updating user information for user {UserId}.",
                command.UserId);
            return Result.Fail(ErrorMessage.Exception, ResultCode.InternalError);
        }
    }
}