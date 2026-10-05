using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.Sales.Register
{
    public interface IRegisterSaleUseCase
    {
        Task<ResponseRegisteredSaleJson> Execute(RequestRegisterSaleJson request);
    }
}
