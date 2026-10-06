using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.Sellers.GetAll
{
    public interface IGetSellersUseCase
    {
        Task<List<ResponseSellerJson>> Execute();
    }
}
