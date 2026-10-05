using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Repositories
{
    internal class SaleRepository(DesafioTargetSistemasDbContext dbContext) : ISaleRepository
    {
        private readonly DesafioTargetSistemasDbContext _dbContext = dbContext;

        public async Task Add(Sale sale) => await _dbContext.Sales.AddAsync(sale);
        public async Task<List<Sale>> GetAll() => await _dbContext.Sales.AsNoTracking()
                                                                        .Include(s => s.Seller)
                                                                        .OrderByDescending(s => s.SaleDate)
                                                                        .ThenByDescending(s => s.Id)
                                                                        .ToListAsync();
    }
}
