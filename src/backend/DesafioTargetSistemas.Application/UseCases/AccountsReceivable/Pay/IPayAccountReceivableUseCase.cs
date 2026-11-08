using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.AccountsReceivable.Pay;

public interface IPayAccountReceivableUseCase
{
    Task<ResponsePaidAccountReceivableJson> Execute(int id);
}
