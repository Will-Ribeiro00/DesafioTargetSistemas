using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DesafioTargetSistemas.Application.UseCases.Products.GetAll;
using DesafioTargetSistemas.Domain.Entities;
using Shouldly;

namespace UseCase.Test.Products.GetAll;

public class GetProductsUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        // Arrange
        List<Product> products = [ProductBuilder.Build(), ProductBuilder.Build()];
        var useCase = CreateUseCase(products);

        // Act
        var result = await useCase.Execute();

        // Assert
        result.Count.ShouldBe(products.Count);

        for (var i = 0; i < products.Count; i++)
        {
            result[i].Code.ShouldBe(products[i].Code);
            result[i].Description.ShouldBe(products[i].Description);
            result[i].Price.ShouldBe(products[i].Price);
            result[i].CurrentStock.ShouldBe(products[i].CurrentStock);
        }
    }

    [Fact]
    public async Task SuccessEmpty()
    {
        // Arrange
        var useCase = CreateUseCase([]);

        // Act
        var result = await useCase.Execute();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    private static GetProductsUseCase CreateUseCase(List<Product> products)
    {
        var repository = new ProductRepositoryBuilder();
        repository.GetAll(products);

        return new GetProductsUseCase(repository.Build());
    }
}