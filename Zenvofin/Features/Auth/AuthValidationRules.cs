using FluentValidation;

namespace Zenvofin.Features.Auth;

public static class AuthValidationRules
{
    extension<T>(IRuleBuilder<T, string> ruleBuilder)
    {
        public IRuleBuilderOptions<T, string> EmailRules()
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(256).WithMessage("Email cannot be longer than 256 characters.")
                .EmailAddress().WithMessage("Invalid email format.")
                .Must(x => x == x?.Trim()).WithMessage("Email cannot contain leading or trailing whitespaces.");
        }

        public IRuleBuilderOptions<T, string> PasswordRules()
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .MaximumLength(32).WithMessage("Password cannot be longer than 32 characters.")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")
                .WithMessage(
                    "Password must contain at least one lowercase letter, one uppercase letter, and one digit.");
        }

        public IRuleBuilderOptions<T, string> UserNameRules()
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                .MaximumLength(50).WithMessage("Name cannot be longer than 50 characters.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Name can only contain letters and spaces.")
                .Must(x => x == x?.Trim()).WithMessage("Name cannot contain leading or trailing whitespaces.");
        }
    }

    extension<T>(IRuleBuilder<T, Guid> ruleBuilder)
    {
        public IRuleBuilderOptions<T, Guid> DeviceIdRules()
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Device id is required.");
        }
    }
}