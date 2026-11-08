using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Domain.Services;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;

namespace DesafioTargetSistemas.Application.UseCases.AccountsReceivable.Pay;

public class PayAccountReceivableUseCase : IPayAccountReceivableUseCase
{
    private readonly IAccountReceivableRepository _accountReceivableRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PayAccountReceivableUseCase(IAccountReceivableRepository accountReceivableRepository, IUnitOfWork unitOfWork)
    {
        _accountReceivableRepository = accountReceivableRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponsePaidAccountReceivableJson> Execute(int id)
    {
        var account = await _accountReceivableRepository.GetById(id)
            ?? throw new NotFoundException(ResourceMessageException.ACCOUNT_RECEIVABLE_NOT_FOUND);

        if (account.PaymentDate is not null)
            throw new ConflictException(ResourceMessageException.ACCOUNT_RECEIVABLE_ALREADY_PAID);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysOverdue = InterestCalculator.DaysOverdue(account.DueDate, today);
        var interest = InterestCalculator.Calculate(account.Amount, daysOverdue);

        account.PaymentDate = today;
        account.PaidAmount = account.Amount + interest;

        _accountReceivableRepository.Update(account);
        await _unitOfWork.Commit();

        return new ResponsePaidAccountReceivableJson
        {
            PaymentDate = account.PaymentDate.Value,
            PaidAmount = account.PaidAmount.Value
        };
    }
}
