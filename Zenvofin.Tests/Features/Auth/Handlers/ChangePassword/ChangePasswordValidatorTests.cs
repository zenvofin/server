using FluentValidation.TestHelper;
using Zenvofin.Features.Auth.Handlers.ChangePassword;
using static Zenvofin.Tests.Features.Auth.TestConstants;

namespace Zenvofin.Tests.Features.Auth.Handlers.ChangePassword;

public class ChangePasswordValidatorTests
{
    private readonly ChangePasswordValidator _validator = new();

    #region Valid Request Tests

    [Fact]
    public void Should_Not_Have_Any_Errors_When_Request_Is_Valid()
    {
        ChangePasswordRequest request = new(ValidPassword, "NewValidPass1");
        TestValidationResult<ChangePasswordRequest>? result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region CurrentPassword Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_CurrentPassword_Is_Empty(string? currentPassword)
    {
        ChangePasswordRequest request = new(currentPassword!, "NewValidPass1");
        TestValidationResult<ChangePasswordRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }

    [Theory]
    [InlineData("any")]
    [InlineData("anypassword")]
    [InlineData("ValidPass1")]
    public void Should_Not_Have_Error_When_CurrentPassword_Is_Not_Empty(string currentPassword)
    {
        ChangePasswordRequest request = new(currentPassword, "NewValidPass1");
        TestValidationResult<ChangePasswordRequest>? result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.CurrentPassword);
    }

    #endregion

    #region NewPassword Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_NewPassword_Is_Empty(string? newPassword)
    {
        ChangePasswordRequest request = new(ValidPassword, newPassword!);
        TestValidationResult<ChangePasswordRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Theory]
    [InlineData("Short1")]
    [InlineData("Abc1234")]
    public void Should_Have_Error_When_NewPassword_Is_Less_Than_8_Characters(string newPassword)
    {
        ChangePasswordRequest request = new(ValidPassword, newPassword);
        TestValidationResult<ChangePasswordRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Should_Have_Error_When_NewPassword_Exceeds_32_Characters()
    {
        string longPassword = "Aa1" + new string('a', 30); // 33 characters
        ChangePasswordRequest request = new(ValidPassword, longPassword);
        TestValidationResult<ChangePasswordRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Theory]
    [InlineData("alllowercase1")]
    [InlineData("ALLUPPERCASE1")]
    [InlineData("NoDigitsHere")]
    [InlineData("12345678")]
    public void Should_Have_Error_When_NewPassword_Does_Not_Meet_Complexity_Requirements(string newPassword)
    {
        ChangePasswordRequest request = new(ValidPassword, newPassword);
        TestValidationResult<ChangePasswordRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Theory]
    [InlineData("ValidPass1")]
    [InlineData("Password123")]
    [InlineData("MyS3cureP@ss")]
    public void Should_Not_Have_Error_When_NewPassword_Is_Valid(string newPassword)
    {
        ChangePasswordRequest request = new(ValidPassword, newPassword);
        TestValidationResult<ChangePasswordRequest>? result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }

    #endregion
}