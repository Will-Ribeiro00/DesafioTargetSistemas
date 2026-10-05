using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.StockMovements.Register
{
    public interface IRegisterStockMovementUseCase
    {
        Task<ResponseRegisteredStockMovementJson> Execute(RequestRegisterStockMovementJson request);
    }
}
