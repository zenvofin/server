namespace Zenvofin.Features.Auth.Handlers.RefreshToken;

public sealed record RefreshTokenCommand(Guid UserId, Guid DeviceId);