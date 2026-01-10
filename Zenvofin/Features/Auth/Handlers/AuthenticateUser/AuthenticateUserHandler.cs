using Microsoft.AspNetCore.Identity;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Features.Auth.Handlers.RefreshToken;
using Zenvofin.Features.Auth.Handlers.RegisterUser;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.AuthenticateUser;

public sealed class AuthenticateUserHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ILogger<RegisterUserHandler> logger)
{
    public async Task<Result<RefreshTokenCommand>> Handle(AuthenticateUserCommand request)
    {
        try
        {
            User? user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return Result<RefreshTokenCommand>.Fail("Invalid email or password.", ResultCode.Unauthorized);

            if (await userManager.IsLockedOutAsync(user))
            {
                logger.LogWarning("User {UserId} is locked out.", user.Id);
                return Result<RefreshTokenCommand>.Fail(
                    "Account is locked due to multiple failed login attempts.",
                    ResultCode.Locked);
            }

            SignInResult result = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);

            if (result.IsLockedOut)
            {
                logger.LogWarning("User {UserId} has been locked out.", user.Id);
                return Result<RefreshTokenCommand>.Fail(
                    "Account is locked due to multiple failed login attempts.",
                    ResultCode.Unauthorized);
            }

            if (!result.Succeeded)
            {
                logger.LogInformation("User {UserId} has entered wrong password.", user.Id);
                return Result<RefreshTokenCommand>.Fail("Invalid email or password.", ResultCode.Unauthorized);
            }

            logger.LogInformation("User {UserId} authenticated successfully.", user.Id);

            RefreshTokenCommand refreshTokenCommandEvent = new(user.Id, request.DeviceId);

            return Result<RefreshTokenCommand>.Success(refreshTokenCommandEvent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating a new user.");
            return Result<RefreshTokenCommand>.Fail(ErrorMessage.Exception, ResultCode.InternalError);
        }
    }
}