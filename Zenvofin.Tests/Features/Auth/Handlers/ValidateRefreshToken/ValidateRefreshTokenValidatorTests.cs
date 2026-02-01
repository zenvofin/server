using FluentValidation.TestHelper;
using Zenvofin.Features.Auth.Handlers.ValidateRefreshToken;
using static Zenvofin.Tests.Features.Auth.TestConstants;

namespace Zenvofin.Tests.Features.Auth.Handlers.ValidateRefreshToken;

public class ValidateRefreshTokenValidatorTests
{
    private readonly ValidateRefreshTokenValidator _validator = new();

    #region Valid Request Tests

    [Fact]
    public void Should_Not_Have_Any_Errors_When_Request_Is_Valid()
    {
        ValidateRefreshTokenRequest request = new(ValidRefreshToken);
        TestValidationResult<ValidateRefreshTokenRequest>? result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region RefreshToken Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_RefreshToken_Is_Empty(string? refreshToken)
    {
        ValidateRefreshTokenRequest request = new(refreshToken!);
        TestValidationResult<ValidateRefreshTokenRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Theory]
    [InlineData("short")]
    [InlineData("12345678901234567890123456789012345678901234567890")] // 50 characters - too long
    [InlineData("1234567890123456789012345678901234567890123")] // 43 characters - too short
    public void Should_Have_Error_When_RefreshToken_Is_Not_44_Characters(string refreshToken)
    {
        ValidateRefreshTokenRequest request = new(refreshToken);
        TestValidationResult<ValidateRefreshTokenRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }

    [Theory]
    [InlineData("12345678901234567890123456789012345678901234")] // Exactly 44 characters
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa=")] // Base64-like 44 characters
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdef==")] // Mixed 44 characters
    public void Should_Not_Have_Error_When_RefreshToken_Is_Exactly_44_Characters(string refreshToken)
    {
        ValidateRefreshTokenRequest request = new(refreshToken);
        TestValidationResult<ValidateRefreshTokenRequest>? result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.RefreshToken);
    }

    #endregion
}