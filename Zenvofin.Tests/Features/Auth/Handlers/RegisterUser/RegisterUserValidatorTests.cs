using FluentValidation.TestHelper;
using Zenvofin.Features.Auth.Handlers.RegisterUser;
using static Zenvofin.Tests.Features.Auth.TestConstants;

namespace Zenvofin.Tests.Features.Auth.Handlers.RegisterUser;

public class RegisterUserValidatorTests
{
    private readonly RegisterUserValidator _validator = new();

    #region Valid Command Tests

    [Fact]
    public void Should_Not_Have_Any_Errors_When_Command_Is_Valid()
    {
        RegisterUserCommand command = new(ValidEmail, ValidName, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Email Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_Email_Is_Empty(string? email)
    {
        RegisterUserCommand command = new(email!, ValidName, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    [InlineData("invalid")]
    public void Should_Have_Error_When_Email_Is_Invalid_Format(string email)
    {
        RegisterUserCommand command = new(email, ValidName, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_256_Characters()
    {
        string longEmail = new string('a', 256) + "@test.com";
        RegisterUserCommand command = new(longEmail, ValidName, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(" test@example.com")]
    [InlineData("test@example.com ")]
    [InlineData(" test@example.com ")]
    public void Should_Have_Error_When_Email_Has_Leading_Or_Trailing_Whitespace(string email)
    {
        RegisterUserCommand command = new(email, ValidName, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.org")]
    [InlineData("user+tag@domain.co.uk")]
    public void Should_Not_Have_Error_When_Email_Is_Valid(string email)
    {
        RegisterUserCommand command = new(email, ValidName, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    #endregion

    #region Name Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_Name_Is_Empty(string? name)
    {
        RegisterUserCommand command = new(ValidEmail, name!, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("A")]
    public void Should_Have_Error_When_Name_Is_Less_Than_2_Characters(string name)
    {
        RegisterUserCommand command = new(ValidEmail, name, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_50_Characters()
    {
        string longName = new('a', 51); // 51 characters
        RegisterUserCommand command = new(ValidEmail, longName, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("John123")]
    [InlineData("John@Doe")]
    [InlineData("John_Doe")]
    [InlineData("John-Doe")]
    public void Should_Have_Error_When_Name_Contains_Invalid_Characters(string name)
    {
        RegisterUserCommand command = new(ValidEmail, name, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(" John")]
    [InlineData("John ")]
    [InlineData(" John ")]
    public void Should_Have_Error_When_Name_Has_Leading_Or_Trailing_Whitespace(string name)
    {
        RegisterUserCommand command = new(ValidEmail, name, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("Jo")]
    [InlineData("John")]
    [InlineData("John Doe")]
    [InlineData("Mary Jane Watson")]
    public void Should_Not_Have_Error_When_Name_Is_Valid(string name)
    {
        RegisterUserCommand command = new(ValidEmail, name, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Password Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_Password_Is_Empty(string? password)
    {
        RegisterUserCommand command = new(ValidEmail, ValidName, password!, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("Short1")]
    [InlineData("Abc1234")]
    public void Should_Have_Error_When_Password_Is_Less_Than_8_Characters(string password)
    {
        RegisterUserCommand command = new(ValidEmail, ValidName, password, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Exceeds_32_Characters()
    {
        string longPassword = "Aa1" + new string('a', 30); // 33 characters
        RegisterUserCommand command = new(ValidEmail, ValidName, longPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("alllowercase1")]
    [InlineData("ALLUPPERCASE1")]
    [InlineData("NoDigitsHere")]
    [InlineData("12345678")]
    public void Should_Have_Error_When_Password_Does_Not_Meet_Complexity_Requirements(string password)
    {
        RegisterUserCommand command = new(ValidEmail, ValidName, password, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("ValidPass1")]
    [InlineData("Password123")]
    [InlineData("MyS3cureP@ss")]
    public void Should_Not_Have_Error_When_Password_Is_Valid(string password)
    {
        RegisterUserCommand command = new(ValidEmail, ValidName, password, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    #endregion

    #region DeviceId Validation Tests

    [Fact]
    public void Should_Have_Error_When_DeviceId_Is_Empty()
    {
        RegisterUserCommand command = new(ValidEmail, ValidName, ValidPassword, Guid.Empty);
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DeviceId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_DeviceId_Is_Valid()
    {
        RegisterUserCommand command = new(ValidEmail, ValidName, ValidPassword, Guid.NewGuid());
        TestValidationResult<RegisterUserCommand>? result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.DeviceId);
    }

    #endregion
}