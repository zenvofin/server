using FastEndpoints;
using Wolverine;
using Zenvofin.Features.Auth.Handlers.AccessToken;
using Zenvofin.Features.Auth.Handlers.RefreshToken;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;
using Zenvofin.Shared.UserInformation;

namespace Zenvofin.Features.Auth.Handlers.ValidateRefreshToken;

public class ValidateRefreshTokenEndpoint(IMessageBus messageBus) : Endpoint<ValidateRefreshTokenRequest, Result>
{
    public override void Configure()
    {
        Post("/auth/refresh");
        Version(1);
        Throttle(10, 60, HeaderConstants.ClientId);
    }

    public override async Task HandleAsync(ValidateRefreshTokenRequest req, CancellationToken ct)
    {
        if (User.TryGetClaims(out UserClaims claims))
        {
            ValidateRefreshTokenCommand command = new(req.RefreshToken, claims.UserId, claims.DeviceId);

            Result<RefreshTokenCommand> refreshTokenEvent =
                await messageBus.InvokeAsync<Result<RefreshTokenCommand>>(command, ct);

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
        else
        {
            await Send.SendAsync(Result.Fail(ErrorMessage.InvalidClaims, ResultCode.Unauthorized), ct);
        }
    }
}