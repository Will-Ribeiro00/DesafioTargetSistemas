using CommonTestUtilities.Requests;
using DesafioTargetSistemas.Application.UseCases.StockMovements.Register;
using DesafioTargetSistemas.Domain.Enums;
using DesafioTargetSistemas.Domain.ValueObjects;
using DesafioTargetSistemas.Exception;
using Shouldly;

namespace Validator.Test.StockMovements.Register;

public class RegisterStockMovementValidatorTest
{
    [Fact]
    public void Success()
    {
        // Arrange
        var validator = new RegisterStockMovementValidator();
        var request = RequestRegisterStockMovementJsonBuilder.Build();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void SuccessDescriptionNull()
    {
        // Arrange
        var validator = new RegisterStockMovementValidator();
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Description = null;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void ErrorCodeEmpty()
    {
        // Arrange
        var validator = new RegisterStockMovementValidator();
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Code = string.Empty;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.PRODUCT_CODE_REQUIRED));
    }

    [Fact]
    public void ErrorCodeTooLong()
    {
        // Arrange
        var validator = new RegisterStockMovementValidator();
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Code = new string('1', DesafioTargetSistemasRuleConstants.PRODUCT_CODE_MAX_LENGTH + 1);

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.PRODUCT_CODE_TOO_LONG));
    }

    [Fact]
    public void ErrorDescriptionTooLong()
    {
        // Arrange
        var validator = new RegisterStockMovementValidator();
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Description = new string('a', DesafioTargetSistemasRuleConstants.STOCK_MOVEMENT_DESCRIPTION_MAX_LENGTH + 1);

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.DESCRIPTION_TOO_LONG));
    }

    [Fact]
    public void ErrorInvalidType()
    {
        // Arrange
        var validator = new RegisterStockMovementValidator();
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Type = (StockMovementType)99;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INVALID_MOVEMENT_TYPE));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ErrorInvalidQuantity(int quantity)
    {
        // Arrange
        var validator = new RegisterStockMovementValidator();
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Quantity = quantity;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INVALID_QUANTITY));
    }
}
