using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Domain.Services;

namespace DesafioTargetSistemas.Application.UseCases.AccountsReceivable.GetPaid;

public class GetPaidAccountsReceivableUseCase(IAccountReceivableRepository accountReceivableRepository) : IGetPaidAccountsReceivableUseCase
{
    private readonly IAccountReceivableRepository _accountReceivableRepository = accountReceivableRepository;

    public async Task<List<ResponsePaidAccountReceivableItemJson>> Execute()
    {
        var accounts = await _accountReceivableRepository.GetPaid();

        return [.. accounts.Select(a => new ResponsePaidAccountReceivableItemJson
        {
            SaleId = a.SaleId,
            Amount = a.Amount,
            DueDate = a.DueDate,
            PaymentDate = a.PaymentDate!.Value,
            DaysOverdue = InterestCalculator.DaysOverdue(a.DueDate, a.PaymentDate!.Value),
            Interest = a.PaidAmount!.Value - a.Amount,
            PaidAmount = a.PaidAmount!.Value
        })];
    }
}
