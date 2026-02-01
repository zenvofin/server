using FluentValidation.TestHelper;
using Zenvofin.Features.Auth.Handlers.UpdateUserInformation;
using static Zenvofin.Tests.Features.Auth.TestConstants;

namespace Zenvofin.Tests.Features.Auth.Handlers.UpdateUserInformation;

public class UpdateUserInformationValidatorTests
{
    private readonly UpdateUserInformationValidator _validator = new();

    #region Valid Request Tests

    [Fact]
    public void Should_Not_Have_Any_Errors_When_Request_Is_Valid()
    {
        UpdateUserInformationRequest request = new(ValidEmail, ValidName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion

    #region Email Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_Email_Is_Empty(string? email)
    {
        UpdateUserInformationRequest request = new(email!, ValidName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    [InlineData("invalid")]
    public void Should_Have_Error_When_Email_Is_Invalid_Format(string email)
    {
        UpdateUserInformationRequest request = new(email, ValidName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_256_Characters()
    {
        string longEmail = new string('a', 256) + "@test.com";
        UpdateUserInformationRequest request = new(longEmail, ValidName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(" test@example.com")]
    [InlineData("test@example.com ")]
    [InlineData(" test@example.com ")]
    public void Should_Have_Error_When_Email_Has_Leading_Or_Trailing_Whitespace(string email)
    {
        UpdateUserInformationRequest request = new(email, ValidName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.org")]
    [InlineData("user+tag@domain.co.uk")]
    public void Should_Not_Have_Error_When_Email_Is_Valid(string email)
    {
        UpdateUserInformationRequest request = new(email, ValidName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    #endregion

    #region UserName Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_UserName_Is_Empty(string? userName)
    {
        UpdateUserInformationRequest request = new(ValidEmail, userName!);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Theory]
    [InlineData("A")]
    public void Should_Have_Error_When_UserName_Is_Less_Than_2_Characters(string userName)
    {
        UpdateUserInformationRequest request = new(ValidEmail, userName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void Should_Have_Error_When_UserName_Exceeds_50_Characters()
    {
        string longUserName = new('a', 51); // 51 characters
        UpdateUserInformationRequest request = new(ValidEmail, longUserName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Theory]
    [InlineData("John123")]
    [InlineData("John@Doe")]
    [InlineData("John_Doe")]
    [InlineData("John-Doe")]
    public void Should_Have_Error_When_UserName_Contains_Invalid_Characters(string userName)
    {
        UpdateUserInformationRequest request = new(ValidEmail, userName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Theory]
    [InlineData(" John")]
    [InlineData("John ")]
    [InlineData(" John ")]
    public void Should_Have_Error_When_UserName_Has_Leading_Or_Trailing_Whitespace(string userName)
    {
        UpdateUserInformationRequest request = new(ValidEmail, userName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Theory]
    [InlineData("Jo")]
    [InlineData("John")]
    [InlineData("John Doe")]
    [InlineData("Mary Jane Watson")]
    public void Should_Not_Have_Error_When_UserName_Is_Valid(string userName)
    {
        UpdateUserInformationRequest request = new(ValidEmail, userName);
        TestValidationResult<UpdateUserInformationRequest>? result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.UserName);
    }

    #endregion
}