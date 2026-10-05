using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Repositories
{
    internal class StockMovementRepository(DesafioTargetSistemasDbContext dbContext) : IStockMovementRepository
    {
        private readonly DesafioTargetSistemasDbContext _dbContext = dbContext;

        public async Task Add(StockMovement movement) =>  await _dbContext.StockMovements.AddAsync(movement);
    }
}
