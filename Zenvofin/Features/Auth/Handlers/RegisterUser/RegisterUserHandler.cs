using Microsoft.AspNetCore.Identity;
using Wolverine;
using Zenvofin.Features.Auth.Data;
using Zenvofin.Features.Auth.Handlers.RefreshToken;
using Zenvofin.Shared;

namespace Zenvofin.Features.Auth.Handlers.RegisterUser;

public sealed class RegisterUserHandler(UserManager<User> userManager)
{
    public async Task<(HandlerContinuation, Result<RefreshTokenCommand>)> Handle(RegisterUserCommand request)
    {
        try
        {
            Guid userId = Guid.NewGuid();

            User user = new() { Id = userId, Email = request.Email };

            IdentityResult result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                Result<RefreshTokenCommand> failResult =
                    Result<RefreshTokenCommand>.Fail(result.Errors.Select(e => e.Description).ToList());
                return (HandlerContinuation.Stop, failResult);
            }

            RefreshTokenCommand refreshTokenCommandEvent = new(userId, request.DeviceId);

            return (HandlerContinuation.Continue, Result<RefreshTokenCommand>.Success(refreshTokenCommandEvent));
        }
        catch (Exception)
        {
            Result<RefreshTokenCommand> errorResult = Result<RefreshTokenCommand>.Fail(ErrorMessage.Exception);
            return (HandlerContinuation.Stop, errorResult);
        }
    }
}