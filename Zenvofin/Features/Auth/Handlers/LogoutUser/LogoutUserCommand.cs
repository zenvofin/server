namespace Zenvofin.Features.Auth.Handlers.LogoutUser;

public sealed record LogoutUserCommand(Guid UserId, Guid DeviceId);