using FastEndpoints;
using Wolverine;
using Zenvofin.Features.Auth.Handlers.AccessToken;
using Zenvofin.Features.Auth.Handlers.RefreshToken;
using Zenvofin.Shared.Result;

namespace Zenvofin.Features.Auth.Handlers.AuthenticateUser;

public class AuthenticateUserEndpoint(IMessageBus messageBus) : Endpoint<AuthenticateUserCommand, Result>
{
    public override void Configure()
    {
        Post("/auth/login");
        Version(1);
        Throttle(20, 60);
        AllowAnonymous();
    }

    public override async Task HandleAsync(AuthenticateUserCommand req, CancellationToken ct)
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