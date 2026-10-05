using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Repositories
{
    public interface ISaleRepository
    {
        Task Add(Sale sale);
        Task<List<Sale>> GetAll();
    }
}
