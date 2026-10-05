using DesafioTargetSistemas.Domain.Repositories;

namespace DesafioTargetSistemas.Infrastructure.DataAccess
{
    internal class UnitOfWork(DesafioTargetSistemasDbContext dbContext) : IUnitOfWork
    {
        private readonly DesafioTargetSistemasDbContext _dbContext = dbContext;

        public async Task Commit() => await _dbContext.SaveChangesAsync();
    }
}
