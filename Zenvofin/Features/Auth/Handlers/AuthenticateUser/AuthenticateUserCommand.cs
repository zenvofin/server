namespace Zenvofin.Features.Auth.Handlers.AuthenticateUser;

public sealed record AuthenticateUserCommand(string Email, string Password, Guid DeviceId);