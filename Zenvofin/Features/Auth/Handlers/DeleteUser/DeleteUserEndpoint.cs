using FastEndpoints;
using Wolverine;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;
using Zenvofin.Shared.UserInformation;

namespace Zenvofin.Features.Auth.Handlers.DeleteUser;

public class DeleteUserEndpoint(IMessageBus messageBus) : EndpointWithoutRequest<Result>
{
    public override void Configure()
    {
        Delete("/auth/user");
        Version(1);
        Throttle(30, 60, HeaderConstants.ClientId);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (User.TryGetClaims(out UserClaims claims))
        {
            DeleteUserCommand command = new(claims.UserId);

            Result result = await messageBus.InvokeAsync<Result>(command, ct);

            await Send.SendAsync(result, ct);
        }
        else
        {
            await Send.SendAsync(Result.Fail(ErrorMessage.InvalidClaims, ResultCode.Unauthorized), ct);
        }
    }
}