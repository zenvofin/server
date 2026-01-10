using FastEndpoints;

namespace Zenvofin.Features.Auth.Handlers.AuthenticateUser;

public class AuthenticateUserValidator : Validator<AuthenticateUserCommand>
{
    public AuthenticateUserValidator()
    {
        RuleFor(x => x.Email).EmailRules();

        RuleFor(x => x.Password).PasswordRules();

        RuleFor(x => x.DeviceId).DeviceIdRules();
    }
}