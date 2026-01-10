namespace Zenvofin.Features.Auth.Handlers.ValidateRefreshToken;

public sealed record ValidateRefreshTokenCommand(string RefreshToken, Guid UserId, Guid DeviceId);