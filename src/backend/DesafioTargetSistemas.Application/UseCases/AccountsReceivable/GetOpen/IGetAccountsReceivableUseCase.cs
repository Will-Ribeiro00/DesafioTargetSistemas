using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.AccountsReceivable.GetOpen
{
    public interface IGetAccountsReceivableUseCase
    {
        Task<List<ResponseAccountReceivableJson>> Execute();
    }
}
