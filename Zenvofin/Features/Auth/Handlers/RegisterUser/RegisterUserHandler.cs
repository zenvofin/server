using Microsoft.AspNetCore.Identity;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Features.Auth.Handlers.RefreshToken;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.RegisterUser;

public sealed class RegisterUserHandler(UserManager<User> userManager, ILogger<RegisterUserHandler> logger)
{
    public async Task<Result<RefreshTokenCommand>> Handle(RegisterUserCommand request)
    {
        try
        {
            Guid userId = Guid.NewGuid();

            User user = new() { Id = userId, Email = request.Email, UserName = request.Name };

            IdentityResult result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                List<string> errors = result.Errors.Select(e => e.Description).ToList();

                return Result<RefreshTokenCommand>.Fail(errors);
            }

            logger.LogInformation("User {UserId} has been created successfully.", userId);

            RefreshTokenCommand refreshTokenCommandEvent = new(userId, request.DeviceId);

            return Result<RefreshTokenCommand>.Success(refreshTokenCommandEvent);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating a new user.");
            return Result<RefreshTokenCommand>.Fail(ErrorMessage.Exception, ResultCode.InternalError);
        }
    }
}