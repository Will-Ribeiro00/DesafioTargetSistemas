using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioTargetSistemas.Infrastructure.DataAccess.Repositories
{
    internal class AppUserRepository(DesafioTargetSistemasDbContext dbContext) : IAppUserRepository
    {
        private readonly DesafioTargetSistemasDbContext _dbContext = dbContext;

        public async Task<AppUser?> GetByEmail(string email)
        {
            return await _dbContext.AppUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
