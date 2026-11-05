using CommonTestUtilities.Requests;
using DesafioTargetSistemas.Application.UseCases.Sellers.GetCommissions;
using DesafioTargetSistemas.Exception;
using Shouldly;

namespace Validator.Test.Sellers.GetCommissions;

public class GetSellerCommissionsValidatorTest
{
    [Fact]
    public void Success()
    {
        // Arrange
        var validator = new GetSellerCommissionsValidator();
        var request = RequestSellerCommissionsJsonBuilder.Build();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void SuccessSameDay()
    {
        // Arrange
        var validator = new GetSellerCommissionsValidator();
        var request = RequestSellerCommissionsJsonBuilder.Build();
        request.From = request.To;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void SuccessWithoutDates()
    {
        // Arrange
        var validator = new GetSellerCommissionsValidator();
        var request = RequestSellerCommissionsJsonBuilder.Build();
        request.From = null;
        request.To = null;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void SuccessOnlyFrom()
    {
        // Arrange
        var validator = new GetSellerCommissionsValidator();
        var request = RequestSellerCommissionsJsonBuilder.Build();
        request.To = null;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void SuccessOnlyTo()
    {
        // Arrange
        var validator = new GetSellerCommissionsValidator();
        var request = RequestSellerCommissionsJsonBuilder.Build();
        request.From = null;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void ErrorFromAfterTo()
    {
        // Arrange
        var validator = new GetSellerCommissionsValidator();
        var request = RequestSellerCommissionsJsonBuilder.Build();
        request.From = request.To!.Value.AddDays(1);

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem();
        result.Errors.ShouldContain(e => e.ErrorMessage.Equals(ResourceMessageException.INVALID_DATE_RANGE));
    }
}