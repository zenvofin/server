namespace Zenvofin.Features.Auth.Handlers.ChangePassword;

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);