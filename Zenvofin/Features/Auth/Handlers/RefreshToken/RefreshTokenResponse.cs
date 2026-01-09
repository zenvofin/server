namespace Zenvofin.Features.Auth.Handlers.RefreshToken;

public sealed record RefreshTokenResponse(Guid UserId, string Token);