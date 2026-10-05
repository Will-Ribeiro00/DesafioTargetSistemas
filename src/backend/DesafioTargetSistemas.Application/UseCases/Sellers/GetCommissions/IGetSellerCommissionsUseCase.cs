using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;

namespace DesafioTargetSistemas.Application.UseCases.Sellers.GetCommissions
{
    public interface IGetSellerCommissionsUseCase
    {
        Task<List<ResponseSellerCommissionJson>> Execute(RequestSellerCommissionsJson request);
    }
}
