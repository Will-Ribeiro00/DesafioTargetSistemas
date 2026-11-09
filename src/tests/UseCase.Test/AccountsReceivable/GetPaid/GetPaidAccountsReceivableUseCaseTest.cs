using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DesafioTargetSistemas.Application.UseCases.AccountsReceivable.GetPaid;
using DesafioTargetSistemas.Domain.Entities;
using Shouldly;

namespace UseCase.Test.AccountsReceivable.GetPaid;

public class GetPaidAccountsReceivableUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        // Arrange
        var paidLate = CreatePaidAccount(
            dueDate: new DateOnly(2026, 10, 1),
            paymentDate: new DateOnly(2026, 10, 4),
            paidAmount: 1075.00m);   // 1.000 + 3 dias × 2,5%

        var paidOnTime = CreatePaidAccount(
            dueDate: new DateOnly(2026, 10, 10),
            paymentDate: new DateOnly(2026, 10, 8),
            paidAmount: 1000.00m);   // pago antes do vencimento, sem juros

        var useCase = CreateUseCase([paidLate, paidOnTime]);

        // Act
        var result = await useCase.Execute();

        // Assert
        result.Count.ShouldBe(2);

        result[0].SaleId.ShouldBe(paidLate.SaleId);
        result[0].Amount.ShouldBe(1000.00m);
        result[0].DueDate.ShouldBe(paidLate.DueDate);
        result[0].PaymentDate.ShouldBe(new DateOnly(2026, 10, 4));
        result[0].DaysOverdue.ShouldBe(3);
        result[0].Interest.ShouldBe(75.00m);
        result[0].PaidAmount.ShouldBe(1075.00m);

        result[1].DaysOverdue.ShouldBe(0);
        result[1].Interest.ShouldBe(0.00m);
        result[1].PaidAmount.ShouldBe(1000.00m);
    }

    [Fact]
    public async Task SuccessInterestComesFromPaidAmount()
    {
        // Arrange
        var account = CreatePaidAccount(
            dueDate: new DateOnly(2026, 10, 1),
            paymentDate: new DateOnly(2026, 10, 4),
            paidAmount: 1090.00m);   // 1.000 + 3 dias × 3% (simulando uma taxa antiga)

        var useCase = CreateUseCase([account]);

        // Act
        var result = await useCase.Execute();

        // Assert
        result[0].Interest.ShouldBe(90.00m);
        result[0].PaidAmount.ShouldBe(1090.00m);
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

    private static AccountReceivable CreatePaidAccount(DateOnly dueDate, DateOnly paymentDate, decimal paidAmount)
    {
        var account = AccountReceivableBuilder.Build(dueDate);
        account.Amount = 1000.00m;
        account.PaymentDate = paymentDate;
        account.PaidAmount = paidAmount;

        return account;
    }

    private static GetPaidAccountsReceivableUseCase CreateUseCase(List<AccountReceivable> accounts)
    {
        var repository = new AccountReceivableRepositoryBuilder();
        repository.GetPaid(accounts);

        return new GetPaidAccountsReceivableUseCase(repository.Build());
    }
}
