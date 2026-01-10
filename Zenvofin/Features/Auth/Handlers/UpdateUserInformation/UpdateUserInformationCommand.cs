namespace Zenvofin.Features.Auth.Handlers.UpdateUserInformation;

public sealed record UpdateUserInformationCommand(Guid UserId, string Email, string UserName);