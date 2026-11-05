using CommonTestUtilities.Dtos;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using DesafioTargetSistemas.Application.UseCases.Sellers.GetCommissions;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Shouldly;

namespace UseCase.Test.Sellers.GetCommissions;

public class GetSellerCommissionsUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        // Arrange
        var summaries = SellerCommissionSummaryBuilder.Collection();
        var request = RequestSellerCommissionsJsonBuilder.Build();

        var saleRepository = new SaleRepositoryBuilder();
        saleRepository.GetCommissionsBySeller(summaries);

        var useCase = CreateUseCase(saleRepository);

        // Act
        var result = await useCase.Execute(request);

        // Assert
        result.Count.ShouldBe(summaries.Count);

        for (var i = 0; i < summaries.Count; i++)
        {
            result[i].SellerId.ShouldBe(summaries[i].SellerId);
            result[i].SellerName.ShouldBe(summaries[i].SellerName);
            result[i].SalesCount.ShouldBe(summaries[i].SalesCount);
            result[i].TotalSold.ShouldBe(summaries[i].TotalSold);
            result[i].TotalCommission.ShouldBe(summaries[i].TotalCommission);
        }
    }

    [Fact]
    public async Task SuccessDatesConversion()
    {
        // Arrange
        var request = RequestSellerCommissionsJsonBuilder.Build();
        request.From = new DateOnly(2026, 10, 1);
        request.To = new DateOnly(2026, 10, 31);

        var saleRepository = new SaleRepositoryBuilder();
        saleRepository.GetCommissionsBySeller([]);

        var useCase = CreateUseCase(saleRepository);

        // Act
        await useCase.Execute(request);

        // Assert
        saleRepository.ReceivedFrom.ShouldBe(new DateTime(2026, 10, 1, 0, 0, 0));
        saleRepository.ReceivedToExclusive.ShouldBe(new DateTime(2026, 11, 1, 0, 0, 0));
    }

    [Fact]
    public async Task SuccessWithoutDates()
    {
        // Arrange
        var request = RequestSellerCommissionsJsonBuilder.Build();
        request.From = null;
        request.To = null;

        var saleRepository = new SaleRepositoryBuilder();
        saleRepository.GetCommissionsBySeller([]);

        var useCase = CreateUseCase(saleRepository);

        // Act
        await useCase.Execute(request);

        // Assert
        saleRepository.ReceivedFrom.ShouldBeNull();
        saleRepository.ReceivedToExclusive.ShouldBeNull();
    }

    [Fact]
    public async Task SuccessEmpty()
    {
        // Arrange
        var request = RequestSellerCommissionsJsonBuilder.Build();

        var saleRepository = new SaleRepositoryBuilder();
        saleRepository.GetCommissionsBySeller([]);

        var useCase = CreateUseCase(saleRepository);

        // Act
        var result = await useCase.Execute(request);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task ErrorInvalidDateRange()
    {
        // Arrange
        var request = RequestSellerCommissionsJsonBuilder.Build();
        request.From = request.To!.Value.AddDays(1);

        var useCase = CreateUseCase(new SaleRepositoryBuilder());

        // Act
        Func<Task> act = async () => await useCase.Execute(request);

        // Assert
        var exception = await act.ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.INVALID_DATE_RANGE);
    }

    private static GetSellerCommissionsUseCase CreateUseCase(SaleRepositoryBuilder saleRepository)
    {
        return new GetSellerCommissionsUseCase(saleRepository.Build());
    }
}