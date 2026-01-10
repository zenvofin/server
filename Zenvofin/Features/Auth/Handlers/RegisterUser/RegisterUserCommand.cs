namespace Zenvofin.Features.Auth.Handlers.RegisterUser;

public sealed record RegisterUserCommand(string Email, string Name, string Password, Guid DeviceId);