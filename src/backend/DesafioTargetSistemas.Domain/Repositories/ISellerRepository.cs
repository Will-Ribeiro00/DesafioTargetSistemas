using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Repositories
{
    public interface ISellerRepository
    {
        Task<Seller?> GetById(int id);
    }
}
