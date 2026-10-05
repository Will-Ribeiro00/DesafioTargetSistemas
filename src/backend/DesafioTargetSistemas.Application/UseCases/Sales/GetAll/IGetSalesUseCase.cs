using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.Sales.GetAll
{
    public interface IGetSalesUseCase
    {
        Task<List<ResponseSaleJson>> Execute();
    }
}
