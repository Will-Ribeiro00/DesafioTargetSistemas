using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using DesafioTargetSistemas.Application.UseCases.AccountsReceivable.Pay;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Services;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Shouldly;

namespace UseCase.Test.AccountsReceivable.Pay;

public class PayAccountReceivableUseCaseTest
{
    private static DateOnly Today => DateOnly.FromDateTime(DateTime.Now);

    [Fact]
    public async Task SuccessOverdue()
    {
        // Arrange
        var account = AccountReceivableBuilder.Build(Today.AddDays(-4));
        account.Id = 1;

        var expectedInterest = InterestCalculator.Calculate(account.Amount, 4);
        var useCase = CreateUseCase(account);

        // Act
        var result = await useCase.Execute(account.Id);

        // Assert
        result.PaymentDate.ShouldBe(Today);
        result.PaidAmount.ShouldBe(account.Amount + expectedInterest);

        account.PaymentDate.ShouldBe(Today);
        account.PaidAmount.ShouldBe(result.PaidAmount);
    }

    [Fact]
    public async Task SuccessNotOverdue()
    {
        // Arrange
        var account = AccountReceivableBuilder.Build(Today.AddDays(10));
        account.Id = 1;

        var useCase = CreateUseCase(account);

        // Act
        var result = await useCase.Execute(account.Id);

        // Assert
        result.PaymentDate.ShouldBe(Today);
        result.PaidAmount.ShouldBe(account.Amount);
    }

    [Fact]
    public async Task ErrorNotFound()
    {
        // Arrange
        var useCase = CreateUseCase();

        // Act
        Func<Task> act = async () => await useCase.Execute(999);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.ACCOUNT_RECEIVABLE_NOT_FOUND);
    }

    [Fact]
    public async Task ErrorAlreadyPaid()
    {
        // Arrange
        var paymentDate = Today.AddDays(-2);

        var account = AccountReceivableBuilder.Build(Today.AddDays(-8));
        account.Id = 1;
        account.PaymentDate = paymentDate;
        account.PaidAmount = account.Amount;

        var paidAmount = account.PaidAmount;
        var useCase = CreateUseCase(account);

        // Act
        Func<Task> act = async () => await useCase.Execute(account.Id);

        // Assert
        var exception = await act.ShouldThrowAsync<ConflictException>();
        exception.GetErrorMessages().ShouldHaveSingleItem();
        exception.GetErrorMessages().ShouldContain(ResourceMessageException.ACCOUNT_RECEIVABLE_ALREADY_PAID);

        account.PaymentDate.ShouldBe(paymentDate);
        account.PaidAmount.ShouldBe(paidAmount);
    }

    private static PayAccountReceivableUseCase CreateUseCase(AccountReceivable? account = null)
    {
        var repository = new AccountReceivableRepositoryBuilder();

        if (account is not null)
            repository.GetById(account);

        return new PayAccountReceivableUseCase(repository.Build(), UnitOfWorkBuilder.Build());
    }
}
