using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.AccountsReceivable.GetPaid;

public interface IGetPaidAccountsReceivableUseCase
{
    Task<List<ResponsePaidAccountReceivableItemJson>> Execute();
}
