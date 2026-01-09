namespace Zenvofin.Features.Auth.Handlers.RegisterUser;

public sealed record RegisterUserCommand(string Email, string Password, Guid DeviceId);