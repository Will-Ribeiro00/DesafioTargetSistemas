using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.AccountsReceivable.GetOpen
{
    public interface IGetOpenAccountsReceivableUseCase
    {
        Task<List<ResponseAccountReceivableJson>> Execute();
    }
}
