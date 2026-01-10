using FluentValidation;

namespace Zenvofin.Features.Auth;

public static class CommonValidationRules
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