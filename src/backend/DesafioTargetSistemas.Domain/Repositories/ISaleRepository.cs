using DesafioTargetSistemas.Domain.Dtos;
using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Repositories
{
    public interface ISaleRepository
    {
        Task Add(Sale sale);
        Task<List<Sale>> GetAll();
        Task<List<SellerCommissionSummary>> GetCommissionsBySeller(DateTime? from, DateTime? toExclusive);
    }
}
