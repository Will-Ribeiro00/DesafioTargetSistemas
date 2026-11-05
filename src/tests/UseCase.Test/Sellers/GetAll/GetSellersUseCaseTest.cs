using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DesafioTargetSistemas.Application.UseCases.Sellers.GetAll;
using DesafioTargetSistemas.Domain.Entities;
using Shouldly;

namespace UseCase.Test.Sellers.GetAll;

public class GetSellersUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        // Arrange
        List<Seller> sellers = [SellerBuilder.Build(), SellerBuilder.Build()];
        var useCase = CreateUseCase(sellers);

        // Act
        var result = await useCase.Execute();

        // Assert
        result.Count.ShouldBe(sellers.Count);

        for (var i = 0; i < sellers.Count; i++)
        {
            result[i].Id.ShouldBe(sellers[i].Id);
            result[i].Name.ShouldBe(sellers[i].Name);
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

    private static GetSellersUseCase CreateUseCase(List<Seller> sellers)
    {
        var repository = new SellerRepositoryBuilder();
        repository.GetAll(sellers);

        return new GetSellersUseCase(repository.Build());
    }
}