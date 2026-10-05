using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Repositories
{
    internal class ProductRepository(DesafioTargetSistemasDbContext dbContext) : IProductRepository
    {
        private readonly DesafioTargetSistemasDbContext _dbContext = dbContext;

        public async Task<Product?> GetByCode(string code) => await _dbContext.Products.FirstOrDefaultAsync(p => p.Code == code);

        public void Update(Product product) => _dbContext.Products.Update(product);
        public async Task<List<Product>> GetAll() => await _dbContext.Products.AsNoTracking().OrderBy(p => p.Code).ToListAsync();
    }
}
