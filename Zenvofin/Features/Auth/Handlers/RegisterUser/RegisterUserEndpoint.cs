using FastEndpoints;
using Wolverine;
using Zenvofin.Shared;

namespace Zenvofin.Features.Auth.Handlers.RegisterUser;

public class RegisterUserEndpoint(IMessageBus messageBus) : Endpoint<RegisterUserCommand, Result<string>>
{
    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterUserCommand req, CancellationToken ct)
    {
        Result<string> result = await messageBus.InvokeAsync<Result<string>>(req, ct);

        await Send.OkAsync(result, ct);
    }
}