using FastEndpoints;
using Wolverine;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;
using Zenvofin.Shared.UserInformation;

namespace Zenvofin.Features.Auth.Handlers.ChangePassword;

public class ChangePasswordEndpoint(IMessageBus messageBus) : Endpoint<ChangePasswordRequest, Result>
{
    public override void Configure()
    {
        Put("/auth/password");
        Version(1);
        Throttle(30, 60, HeaderConstants.ClientId);
    }

    public override async Task HandleAsync(ChangePasswordRequest req, CancellationToken ct)
    {
        if (User.TryGetClaims(out UserClaims claims))
        {
            ChangePasswordCommand command = new(claims.UserId, req.CurrentPassword, req.NewPassword);

            Result result = await messageBus.InvokeAsync<Result>(command, ct);

            await Send.SendAsync(result, ct);
        }
        else
        {
            await Send.SendAsync(Result.Fail(ErrorMessage.InvalidClaims, ResultCode.Unauthorized), ct);
        }
    }
}