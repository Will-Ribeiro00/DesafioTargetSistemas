using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Repositories
{
    internal class SellerRepository(DesafioTargetSistemasDbContext dbContext) : ISellerRepository
    {
        private readonly DesafioTargetSistemasDbContext _dbContext = dbContext;

        public async Task<Seller?> GetById(int id) => await _dbContext.Sellers.FirstOrDefaultAsync(s => s.Id == id);
        public async Task<List<Seller>> GetAll() => await _dbContext.Sellers.AsNoTracking().OrderBy(s => s.Name).ToListAsync();
    }
}
