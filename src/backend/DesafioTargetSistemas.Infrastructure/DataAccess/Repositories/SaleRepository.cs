using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Repositories
{
    internal class SaleRepository(DesafioTargetSistemasDbContext dbContext) : ISaleRepository
    {
        private readonly DesafioTargetSistemasDbContext _dbContext = dbContext;

        public async Task Add(Sale sale) => await _dbContext.Sales.AddAsync(sale);
    }
}
