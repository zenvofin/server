using FastEndpoints;
using Wolverine;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;
using Zenvofin.Shared.UserInformation;

namespace Zenvofin.Features.Auth.Handlers.UpdateUserInformation;

public class UpdateUserInformationEndpoint(IMessageBus messageBus) : Endpoint<UpdateUserInformationRequest, Result>
{
    public override void Configure()
    {
        Put("/auth/user");
        Version(1);
        Throttle(30, 60, HeaderConstants.ClientId);
    }

    public override async Task HandleAsync(UpdateUserInformationRequest req, CancellationToken ct)
    {
        if (User.TryGetClaims(out UserClaims claims))
        {
            UpdateUserInformationCommand command = new(claims.UserId, req.Email, req.UserName);

            Result result = await messageBus.InvokeAsync<Result>(command, ct);

            await Send.SendAsync(result, ct);
        }
        else
        {
            await Send.SendAsync(Result.Fail(ErrorMessage.InvalidClaims, ResultCode.Unauthorized), ct);
        }
    }
}