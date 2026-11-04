using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using DesafioTargetSistemas.Application.UseCases.StockMovements.Register;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Enums;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Shouldly;

namespace UseCase.Test.StockMovements.Register;

public class RegisterStockMovementUseCaseTest
{
    [Fact]
    public async Task SuccessIn()
    {
        // Arrange
        var product = ProductBuilder.Build();
        var initialStock = product.CurrentStock;

        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Code = product.Code;
        request.Type = StockMovementType.In;

        var useCase = CreateUseCase(product);

        // Act
        var result = await useCase.Execute(request);

        // Assert
        result.ShouldNotBeNull();
        result.StockBalance.ShouldBe(initialStock + request.Quantity);
        product.CurrentStock.ShouldBe(result.StockBalance);
    }

    [Fact]
    public async Task SuccessOut()
    {
        // Arrange
        var product = ProductBuilder.Build();
        var initialStock = product.CurrentStock;

        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Code = product.Code;
        request.Type = StockMovementType.Out;
        request.Quantity = initialStock;

        var useCase = CreateUseCase(product);

        // Act
        var result = await useCase.Execute(request);

        // Assert
        result.StockBalance.ShouldBe(0);
        product.CurrentStock.ShouldBe(0);
    }

    [Fact]
    public async Task ErrorProductNotFound()
    {
        // Arrange
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        var useCase = CreateUseCase();

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.PRODUCT_NOT_FOUND);
    }

    [Fact]
    public async Task ErrorInsufficientStock()
    {
        // Arrange
        var product = ProductBuilder.Build();
        var initialStock = product.CurrentStock;

        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Code = product.Code;
        request.Type = StockMovementType.Out;
        request.Quantity = initialStock + 1;

        var useCase = CreateUseCase(product);

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<InsufficientStockException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.INSUFFICIENT_STOCK);
        product.CurrentStock.ShouldBe(initialStock);
    }

    [Fact]
    public async Task ErrorInvalidRequest()
    {
        // Arrange
        var request = RequestRegisterStockMovementJsonBuilder.Build();
        request.Quantity = 0;

        var useCase = CreateUseCase();

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.INVALID_QUANTITY);
    }

    private static RegisterStockMovementUseCase CreateUseCase(Product? product = null)
    {
        var productRepository = new ProductRepositoryBuilder();
        var stockMovementRepository = StockMovementRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();

        if (product is not null)
            productRepository.GetByCode(product);

        return new RegisterStockMovementUseCase(productRepository.Build(), stockMovementRepository, unitOfWork);
    }
}
