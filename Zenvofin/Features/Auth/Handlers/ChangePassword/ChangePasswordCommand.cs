namespace Zenvofin.Features.Auth.Handlers.ChangePassword;

public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);