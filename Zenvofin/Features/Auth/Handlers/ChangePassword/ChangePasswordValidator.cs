using FastEndpoints;
using FluentValidation;

namespace Zenvofin.Features.Auth.Handlers.ChangePassword;

public class ChangePasswordValidator : Validator<ChangePasswordRequest>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword).PasswordRules();
    }
}