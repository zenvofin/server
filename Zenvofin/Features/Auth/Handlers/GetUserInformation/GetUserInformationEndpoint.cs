using FastEndpoints;
using Wolverine;
using Zenvofin.Shared;
using Zenvofin.Shared.Result;
using Zenvofin.Shared.UserInformation;

namespace Zenvofin.Features.Auth.Handlers.GetUserInformation;

public class GetUserInformationEndpoint(IMessageBus messageBus) : EndpointWithoutRequest<Result>
{
    public override void Configure()
    {
        Get("/auth/user");
        Version(1);
        Throttle(60, 60, HeaderConstants.ClientId);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (User.TryGetClaims(out UserClaims claims))
        {
            GetUserInformationCommand command = new(claims.UserId);

            Result<GetUserInformationResponse> result =
                await messageBus.InvokeAsync<Result<GetUserInformationResponse>>(command, ct);

            await Send.SendAsync(result, ct);
        }
        else
        {
            await Send.SendAsync(Result.Fail(ErrorMessage.InvalidClaims, ResultCode.Unauthorized), ct);
        }
    }
}