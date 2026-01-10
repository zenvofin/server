using FastEndpoints;
using FluentValidation;

namespace Zenvofin.Features.Auth.Handlers.ValidateRefreshToken;

public class ValidateRefreshTokenValidator : Validator<ValidateRefreshTokenRequest>
{
    public ValidateRefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .Length(44).WithMessage("Refresh token must be exactly 44 characters.");
    }
}