using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Microsoft.EntityFrameworkCore;

namespace DesafioTargetSistemas.Infrastructure.DataAccess
{
    internal class UnitOfWork(DesafioTargetSistemasDbContext dbContext) : IUnitOfWork
    {
        private readonly DesafioTargetSistemasDbContext _dbContext = dbContext;

        public async Task Commit()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                throw new ConcurrencyConflictException();
            }
        }

        public void Discard() => _dbContext.ChangeTracker.Clear();
    }
}
