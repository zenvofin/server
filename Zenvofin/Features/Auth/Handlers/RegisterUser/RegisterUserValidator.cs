using FastEndpoints;
using FluentValidation;

namespace Zenvofin.Features.Auth.Handlers.RegisterUser;

public class RegisterUserValidator : Validator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email).EmailRules();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
            .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters.")
            .Matches(@"^[a-zA-Z\s]+$").WithMessage("Name can only contain letters and spaces.")
            .Must(x => x == x?.Trim()).WithMessage("Name cannot contain leading or trailing whitespaces.");

        RuleFor(x => x.Password).PasswordRules();

        RuleFor(x => x.DeviceId).DeviceIdRules();
    }
}