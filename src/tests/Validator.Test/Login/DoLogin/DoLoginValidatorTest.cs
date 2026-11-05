using CommonTestUtilities.Requests;
using DesafioTargetSistemas.Application.UseCases.Login.DoLogin;
using DesafioTargetSistemas.Exception;
using Shouldly;

namespace Validator.Test.Login.DoLogin;

public class DoLoginValidatorTest
{
    [Fact]
    public void Success()
    {
        // Arrange
        var validator = new DoLoginValidator();
        var request = RequestLoginJsonBuilder.Build();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ErrorEmailEmpty(string email)
    {
        // Arrange
        var validator = new DoLoginValidator();
        var request = RequestLoginJsonBuilder.Build();
        request.Email = email;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.EMAIL_REQUIRED));
    }

    [Fact]
    public void ErrorEmailInvalid()
    {
        // Arrange
        var validator = new DoLoginValidator();
        var request = RequestLoginJsonBuilder.Build();
        request.Email = "emailinvalido";

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INVALID_EMAIL_FORMAT));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ErrorPasswordEmpty(string password)
    {
        // Arrange
        var validator = new DoLoginValidator();
        var request = RequestLoginJsonBuilder.Build();
        request.Password = password;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.PASSWORD_REQUIRED));
    }
}
