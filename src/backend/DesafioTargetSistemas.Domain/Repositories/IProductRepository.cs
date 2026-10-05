using DesafioTargetSistemas.Domain.Entities;

namespace DesafioTargetSistemas.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByCode(string code);
        void Update(Product product);
    }
}
