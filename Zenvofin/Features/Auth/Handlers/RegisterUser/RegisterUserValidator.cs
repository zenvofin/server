using FastEndpoints;

namespace Zenvofin.Features.Auth.Handlers.RegisterUser;

public class RegisterUserValidator : Validator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email).EmailRules();

        RuleFor(x => x.Name).UserNameRules();

        RuleFor(x => x.Password).PasswordRules();

        RuleFor(x => x.DeviceId).DeviceIdRules();
    }
}