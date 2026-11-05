using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DesafioTargetSistemas.Application.UseCases.Sales.GetAll;
using DesafioTargetSistemas.Domain.Entities;
using Shouldly;

namespace UseCase.Test.Sales.GetAll;

public class GetSalesUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        // Arrange
        var seller = SellerBuilder.Build();

        List<Sale> sales =
        [
            SaleBuilder.Build(seller, new DateTime(2026, 10, 20, 15, 30, 0, DateTimeKind.Utc), 300.00m),
            SaleBuilder.Build(seller, new DateTime(2026, 9, 15, 10, 0, 0, DateTimeKind.Utc), 1000.00m)
        ];

        foreach (var sale in sales)
            sale.Seller = seller;

        var useCase = CreateUseCase(sales);

        // Act
        var result = await useCase.Execute();

        // Assert
        result.Count.ShouldBe(sales.Count);

        for (var i = 0; i < sales.Count; i++)
        {
            result[i].Id.ShouldBe(sales[i].Id);
            result[i].SaleDate.ShouldBe(sales[i].SaleDate);
            result[i].SellerName.ShouldBe(seller.Name);
            result[i].TotalPrice.ShouldBe(sales[i].TotalPrice);
            result[i].CommissionPercentage.ShouldBe(sales[i].CommissionPercentage);
            result[i].CommissionAmount.ShouldBe(sales[i].CommissionAmount);
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

    private static GetSalesUseCase CreateUseCase(List<Sale> sales)
    {
        var repository = new SaleRepositoryBuilder();
        repository.GetAll(sales);

        return new GetSalesUseCase(repository.Build());
    }
}