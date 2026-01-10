using FastEndpoints;
using Wolverine;
using Zenvofin.Features.Auth.Handlers.AccessToken;
using Zenvofin.Features.Auth.Handlers.RefreshToken;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.RegisterUser;

public class RegisterUserEndpoint(IMessageBus messageBus) : Endpoint<RegisterUserCommand, Result>
{
    public override void Configure()
    {
        Post("/auth/register");
        Version(1);
        Throttle(10, 60);
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterUserCommand req, CancellationToken ct)
    {
        Result<RefreshTokenCommand> refreshTokenEvent =
            await messageBus.InvokeAsync<Result<RefreshTokenCommand>>(req, ct);

        if (!refreshTokenEvent.IsSuccess)
        {
            await Send.SendAsync(refreshTokenEvent, ct);
            return;
        }

        Result<AccessTokenCommand> accessTokenEvent =
            await messageBus.InvokeAsync<Result<AccessTokenCommand>>(refreshTokenEvent.Data!, ct);

        if (!accessTokenEvent.IsSuccess)
        {
            await Send.SendAsync(accessTokenEvent, ct);
            return;
        }

        Result<AccessTokenResponse> jwtResult =
            await messageBus.InvokeAsync<Result<AccessTokenResponse>>(accessTokenEvent.Data!, ct);

        await Send.SendAsync(jwtResult, ct);
    }
}