using FastEndpoints;

namespace Zenvofin.Features.Auth.Handlers.UpdateUserInformation;

public class UpdateUserInformationValidator : Validator<UpdateUserInformationRequest>
{
    public UpdateUserInformationValidator()
    {
        RuleFor(x => x.Email).EmailRules();

        RuleFor(x => x.UserName).UserNameRules();
    }
}