using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DesafioTargetSistemas.Application.UseCases.AccountsReceivable.GetOpen;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Services;
using Shouldly;

namespace UseCase.Test.AccountsReceivable.GetOpen;

public class GetOpenAccountsReceivableUseCaseTest
{
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);

    [Fact]
    public async Task Success()
    {
        // Arrange
        List<AccountReceivable> accounts =
        [
            AccountReceivableBuilder.Build(Today.AddDays(-4)),
            AccountReceivableBuilder.Build(Today),
            AccountReceivableBuilder.Build(Today.AddDays(10))
        ];
        int[] expectedDays = [4, 0, 0];

        var useCase = CreateUseCase(accounts);

        // Act
        var result = await useCase.Execute();

        // Assert
        result.Count.ShouldBe(accounts.Count);

        for (var i = 0; i < accounts.Count; i++)
        {
            var expectedInterest = InterestCalculator.Calculate(accounts[i].Amount, expectedDays[i]);

            result[i].SaleId.ShouldBe(accounts[i].SaleId);
            result[i].Amount.ShouldBe(accounts[i].Amount);
            result[i].DueDate.ShouldBe(accounts[i].DueDate);
            result[i].DaysOverdue.ShouldBe(expectedDays[i]);
            result[i].Interest.ShouldBe(expectedInterest);
            result[i].TotalAmount.ShouldBe(accounts[i].Amount + expectedInterest);
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

    private static GetOpenAccountsReceivableUseCase CreateUseCase(List<AccountReceivable> accounts)
    {
        var repository = new AccountReceivableRepositoryBuilder();
        repository.GetOpen(accounts);

        return new GetOpenAccountsReceivableUseCase(repository.Build());
    }
}