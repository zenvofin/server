using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.DeleteUser;

public sealed class DeleteUserHandler(
    UserManager<User> userManager,
    ILogger<DeleteUserHandler> logger)
{
    public async Task<Result> Handle(DeleteUserCommand command)
    {
        try
        {
            User? user = await userManager.FindByIdAsync(command.UserId.ToString());

            if (user is null)
            {
                logger.LogWarning("User not found with ID {UserId}.", command.UserId);
                return Result.Fail("User not found.", ResultCode.NotFound);
            }

            IdentityResult result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                List<string> errors = result.Errors.Select(e => e.Description).ToList();
                logger.LogWarning(
                    "Failed to delete user {UserId}. Errors: {Errors}",
                    command.UserId,
                    string.Join(", ", errors));
                return Result.Fail(errors);
            }

            logger.LogInformation("User {UserId} has been deleted successfully.", command.UserId);

            return Result.Success("User deleted successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while deleting user {UserId}.",
                command.UserId);
            return Result.Fail(ErrorMessage.Exception, ResultCode.InternalError);
        }
    }
}