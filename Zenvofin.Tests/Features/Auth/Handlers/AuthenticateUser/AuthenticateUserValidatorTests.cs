using FluentValidation.TestHelper;
using Zenvofin.Features.Auth.Handlers.AuthenticateUser;
using static Zenvofin.Tests.Features.Auth.TestConstants;

namespace Zenvofin.Tests.Features.Auth.Handlers.AuthenticateUser;

public class AuthenticateUserValidatorTests
{
    private readonly AuthenticateUserValidator _validator = new();

    #region Valid Command Tests

    [Fact]
    public void Should_Not_Have_Any_Errors_When_Command_Is_Valid()
    {
        AuthenticateUserCommand command = new(ValidEmail, ValidPassword, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Email Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_Email_Is_Empty(string? email)
    {
        AuthenticateUserCommand command = new(email!, ValidPassword, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    [InlineData("invalid")]
    public void Should_Have_Error_When_Email_Is_Invalid_Format(string email)
    {
        AuthenticateUserCommand command = new(email, ValidPassword, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_256_Characters()
    {
        string longEmail = new string('a', 256) + "@test.com";
        AuthenticateUserCommand command = new(longEmail, ValidPassword, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(" test@example.com")]
    [InlineData("test@example.com ")]
    [InlineData(" test@example.com ")]
    public void Should_Have_Error_When_Email_Has_Leading_Or_Trailing_Whitespace(string email)
    {
        AuthenticateUserCommand command = new(email, ValidPassword, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.org")]
    [InlineData("user+tag@domain.co.uk")]
    public void Should_Not_Have_Error_When_Email_Is_Valid(string email)
    {
        AuthenticateUserCommand command = new(email, ValidPassword, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    #endregion

    #region Password Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_Password_Is_Empty(string? password)
    {
        AuthenticateUserCommand command = new(ValidEmail, password!, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("Short1")]
    [InlineData("Abc1234")]
    public void Should_Have_Error_When_Password_Is_Less_Than_8_Characters(string password)
    {
        AuthenticateUserCommand command = new(ValidEmail, password, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Exceeds_32_Characters()
    {
        string longPassword = "Aa1" + new string('a', 30); // 33 characters
        AuthenticateUserCommand command = new(ValidEmail, longPassword, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("alllowercase1")]
    [InlineData("ALLUPPERCASE1")]
    [InlineData("NoDigitsHere")]
    [InlineData("12345678")]
    public void Should_Have_Error_When_Password_Does_Not_Meet_Complexity_Requirements(string password)
    {
        AuthenticateUserCommand command = new(ValidEmail, password, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("ValidPass1")]
    [InlineData("Password123")]
    [InlineData("MyS3cureP@ss")]
    public void Should_Not_Have_Error_When_Password_Is_Valid(string password)
    {
        AuthenticateUserCommand command = new(ValidEmail, password, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    #endregion

    #region DeviceId Validation Tests

    [Fact]
    public void Should_Have_Error_When_DeviceId_Is_Empty()
    {
        AuthenticateUserCommand command = new(ValidEmail, ValidPassword, Guid.Empty);
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DeviceId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_DeviceId_Is_Valid()
    {
        AuthenticateUserCommand command = new(ValidEmail, ValidPassword, Guid.NewGuid());
        TestValidationResult<AuthenticateUserCommand>? result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.DeviceId);
    }

    #endregion
}