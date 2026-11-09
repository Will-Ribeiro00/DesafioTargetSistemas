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

        public async Task<List<AccountReceivable>> GetPaid()
        {
            return await _dbContext.AccountReceivables
                .AsNoTracking()
                .Where(a => a.PaymentDate != null)
                .OrderByDescending(a => a.PaymentDate)
                .ThenByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<AccountReceivable?> GetById(int id) => await _dbContext.AccountReceivables.FirstOrDefaultAsync(a => a.Id == id);

        public void Update(AccountReceivable accountReceivable) => _dbContext.AccountReceivables.Update(accountReceivable);
    }
}
