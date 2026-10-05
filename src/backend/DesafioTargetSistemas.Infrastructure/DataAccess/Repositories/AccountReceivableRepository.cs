using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Repositories
{
    internal class AccountReceivableRepository(DesafioTargetSistemasDbContext dbContext) : IAccountReceivableRepository
    {
        private readonly DesafioTargetSistemasDbContext _dbContext = dbContext;

        public async Task<List<AccountReceivable>> GetOpen()
        {
            return await _dbContext.AccountReceivables
                .AsNoTracking()
                .Where(a => a.PaymentDate == null)
                .OrderBy(a => a.DueDate)
                .ThenBy(a => a.Id)
                .ToListAsync();
        }
    }
}
