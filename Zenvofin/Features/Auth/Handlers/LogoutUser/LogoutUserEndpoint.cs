using FastEndpoints;
using Wolverine;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;
using Zenvofin.Shared.UserInformation;

namespace Zenvofin.Features.Auth.Handlers.LogoutUser;

public class LogoutUserEndpoint(IMessageBus messageBus) : EndpointWithoutRequest<Result>
{
    public override void Configure()
    {
        Post("/auth/logout");
        Throttle(10, 60, HeaderConstants.ClientId);
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (User.TryGetClaims(out UserClaims claims))
        {
            LogoutUserCommand command = new(claims.UserId, claims.DeviceId);

            Result result = await messageBus.InvokeAsync<Result>(command, ct);

            await Send.SendAsync(result, ct);
        }
        else
        {
            await Send.SendAsync(Result.Fail("User claims are invalid.", ResultCode.Unauthorized), ct);
        }
    }
}