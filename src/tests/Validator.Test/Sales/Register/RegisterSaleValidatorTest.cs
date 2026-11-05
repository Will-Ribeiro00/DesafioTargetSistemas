using CommonTestUtilities.Requests;
using DesafioTargetSistemas.Application.UseCases.Sales.Register;
using DesafioTargetSistemas.Domain.ValueObjects;
using DesafioTargetSistemas.Exception;
using Shouldly;

namespace Validator.Test.Sales.Register;

public class RegisterSaleValidatorTest
{
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    [Fact]
    public void Success()
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(DesafioTargetSistemasRuleConstants.SALE_MAX_DUE_DAYS)]
    public void SuccessDueDateOnLimits(int days)
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build();
        request.DueDate = Today.AddDays(days);

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ErrorInvalidSellerId(int sellerId)
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build();
        request.SellerId = sellerId;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INVALID_SELLER_ID));
    }

    [Fact]
    public void ErrorItemsEmpty()
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build(0);

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.SALE_ITEMS_REQUIRED));
    }

    [Fact]
    public void ErrorDuplicatedProducts()
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build();
        request.Items[1].Code = request.Items[0].Code;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.DUPLICATED_PRODUCTS_IN_SALE));
    }

    [Fact]
    public void ErrorDuplicatedProductsIgnoringCase()
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build();
        request.Items[0].Code = "abc";
        request.Items[1].Code = "ABC";

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.DUPLICATED_PRODUCTS_IN_SALE));
    }

    [Fact]
    public void ErrorItemCodeEmpty()
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build();
        request.Items[0].Code = string.Empty;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.PRODUCT_CODE_REQUIRED));
    }

    [Fact]
    public void ErrorItemCodeTooLong()
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build();
        request.Items[0].Code = new string('1', DesafioTargetSistemasRuleConstants.PRODUCT_CODE_MAX_LENGTH + 1);

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.PRODUCT_CODE_TOO_LONG));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ErrorItemInvalidQuantity(int quantity)
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build();
        request.Items[0].Quantity = quantity;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INVALID_QUANTITY));
    }

    [Fact]
    public void ErrorDueDateInThePast()
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build();
        request.DueDate = Today.AddDays(-1);

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.DUE_DATE_IN_THE_PAST));
    }

    [Fact]
    public void ErrorDueDateTooFar()
    {
        // Arrange
        var validator = new RegisterSaleValidator();
        var request = RequestRegisterSaleJsonBuilder.Build();
        request.DueDate = Today.AddDays(DesafioTargetSistemasRuleConstants.SALE_MAX_DUE_DAYS + 1);

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.DUE_DATE_TOO_FAR));
    }
}
