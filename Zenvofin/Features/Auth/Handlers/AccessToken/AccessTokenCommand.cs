namespace Zenvofin.Features.Auth.Handlers.AccessToken;

public sealed record AccessTokenCommand(Guid UserId, Guid DeviceId);