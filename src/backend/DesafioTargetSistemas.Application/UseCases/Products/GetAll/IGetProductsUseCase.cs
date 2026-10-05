using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.Products.GetAll
{
    public interface IGetProductsUseCase
    {
        Task<List<ResponseProductJson>> Execute();
    }
}
