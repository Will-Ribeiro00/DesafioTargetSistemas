using DesafioTargetSistemas.Domain.Dtos;
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

        public async Task<List<SellerCommissionSummary>> GetCommissionsBySeller(DateTime? from, DateTime? toExclusive)
        {
            var query = _dbContext.Sales.AsNoTracking();

            if (from.HasValue)
                query = query.Where(s => s.SaleDate >= from.Value);

            if (toExclusive.HasValue)
                query = query.Where(s => s.SaleDate < toExclusive.Value);

            return await query
                .GroupBy(s => new { s.SellerId, s.Seller.Name })
                .OrderBy(g => g.Key.Name)
                .Select(g => new SellerCommissionSummary(
                    g.Key.SellerId,
                    g.Key.Name,
                    g.Count(),
                    g.Sum(s => s.TotalPrice),
                    g.Sum(s => s.CommissionAmount)))
                .ToListAsync();
        }
    }
}
